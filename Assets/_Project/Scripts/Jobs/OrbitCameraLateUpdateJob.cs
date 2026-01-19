using AndrzejKebab.Components;
using Unity.Burst;
using Unity.CharacterController;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace AndrzejKebab.Jobs
{
	[BurstCompile]
	[WithAll(typeof(Simulate))]
	public partial struct OrbitCameraLateUpdateJob : IJobEntity
	{
		public            float        DeltaTime;
		[ReadOnly] public PhysicsWorld PhysicsWorld;

		public            ComponentLookup<LocalToWorld>           LocalToWorldLookup;
		[ReadOnly] public ComponentLookup<CameraTarget>           CameraTargetLookup;
		[ReadOnly] public ComponentLookup<KinematicCharacterBody> KinematicCharacterBodyLookup;

		private void Execute(
			Entity                                                   entity,
			ref OrbitCameraComponent                                 orbitCameraComponent,
			in  OrbitCameraControl                                   cameraControl,
			in  DynamicBuffer<OrbitCameraIgnoredEntityBufferElement> ignoredEntitiesBuffer)
		{
			if (!OrbitCameraUtilities.TryGetCameraTargetInterpolatedWorldTransform(
				     in cameraControl.FollowedCharacterEntity,
				     ref LocalToWorldLookup,
				     ref CameraTargetLookup,
				     out LocalToWorld targetWorldTransform)) return;
			float3     worldTransformUp = targetWorldTransform.Up;
			OrbitCameraUtilities.CalculateCameraRotation(ref worldTransformUp,
			                                             ref orbitCameraComponent.PlanarForward,
			                                             orbitCameraComponent.PitchAngle, out quaternion cameraRotation);

			float3 cameraForward  = math.mul(cameraRotation, math.forward());
			float3 targetPosition = targetWorldTransform.Position;

			// Distance smoothing
			orbitCameraComponent.SmoothedTargetDistance = math.lerp(orbitCameraComponent.SmoothedTargetDistance,
			                                                        orbitCameraComponent.TargetDistance,
			                                                        MathUtilities
				                                                        .GetSharpnessInterpolant(orbitCameraComponent
						                                                         .DistanceMovementSharpness,
					                                                         DeltaTime));

			// Obstruction handling
			// Obstruction detection is handled here, because we have to adjust the obstruction distance
			// to match the interpolated physics body transform (as opposed to the "simulation" transform). Otherwise, a
			// camera getting obstructed by a moving physics body would have visible jitter.
			if (orbitCameraComponent.ObstructionRadius > 0f)
			{
				var obstructionCheckDistance = orbitCameraComponent.SmoothedTargetDistance;

				var collector =
					new CameraObstructionHitsCollector(cameraControl.FollowedCharacterEntity, ignoredEntitiesBuffer,
					                                   cameraForward);
				PhysicsWorld.SphereCastCustom(
				                              targetPosition,
				                              orbitCameraComponent.ObstructionRadius,
				                              -cameraForward,
				                              obstructionCheckDistance,
				                              ref collector,
				                              CollisionFilter.Default,
				                              QueryInteraction.IgnoreTriggers);

				var newObstructedDistance = obstructionCheckDistance;
				if (collector.NumHits > 0)
				{
					newObstructedDistance = obstructionCheckDistance * collector.ClosestHit.Fraction;

					// Redo cast with the interpolated body transform to prevent FixedUpdate jitter in obstruction detection
					if (orbitCameraComponent.PreventFixedUpdateJitter)
					{
						RigidBody hitBody = PhysicsWorld.Bodies[collector.ClosestHit.RigidBodyIndex];
						if (LocalToWorldLookup.TryGetComponent(hitBody.Entity,
						                                       out LocalToWorld hitBodyLocalToWorld))
						{
							// Adjust the rigidbody transform for interpolation, so we can raycast it in that state
							hitBody.WorldFromBody =
								new RigidTransform(quaternion.LookRotationSafe(hitBodyLocalToWorld.Forward,
								                                               hitBodyLocalToWorld.Up),
								                   hitBodyLocalToWorld.Position);

							collector = new CameraObstructionHitsCollector(cameraControl.FollowedCharacterEntity,
							                                               ignoredEntitiesBuffer, cameraForward);
							hitBody.SphereCastCustom(
							                         targetPosition,
							                         orbitCameraComponent.ObstructionRadius,
							                         -cameraForward,
							                         obstructionCheckDistance,
							                         ref collector,
							                         CollisionFilter.Default,
							                         QueryInteraction.IgnoreTriggers);

							if (collector.NumHits > 0)
								newObstructedDistance = obstructionCheckDistance * collector.ClosestHit.Fraction;
						}
					}
				}

				// Update current distance based on obstructed distance
				if (orbitCameraComponent.ObstructedDistance < newObstructedDistance)
					// Move outer
					orbitCameraComponent.ObstructedDistance = math.lerp(orbitCameraComponent.ObstructedDistance,
					                                                    newObstructedDistance,
					                                                    MathUtilities
						                                                    .GetSharpnessInterpolant(orbitCameraComponent
								                                                     .ObstructionOuterSmoothingSharpness,
							                                                     DeltaTime));
				else if (orbitCameraComponent.ObstructedDistance > newObstructedDistance)
					// Move inner
					orbitCameraComponent.ObstructedDistance = math.lerp(orbitCameraComponent.ObstructedDistance,
					                                                    newObstructedDistance,
					                                                    MathUtilities
						                                                    .GetSharpnessInterpolant(orbitCameraComponent
								                                                     .ObstructionInnerSmoothingSharpness,
							                                                     DeltaTime));
			}
			else
			{
				orbitCameraComponent.ObstructedDistance = orbitCameraComponent.SmoothedTargetDistance;
			}

			// Place camera at the final distance (includes smoothing and obstructions)
			OrbitCameraUtilities.CalculateCameraPosition(ref targetPosition, ref cameraRotation,
			                                             orbitCameraComponent.ObstructedDistance, out float3 cameraPosition);

			// Write to LtW
			LocalToWorldLookup[entity] = new LocalToWorld { Value = new float4x4(cameraRotation, cameraPosition) };
		}
	}
}