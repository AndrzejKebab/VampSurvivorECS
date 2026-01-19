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
		[ReadOnly] public ComponentLookup<CameraTargetComponent>           CameraTargetLookup;
		[ReadOnly] public ComponentLookup<KinematicCharacterBody> KinematicCharacterBodyLookup;

		private void Execute(
			Entity                                                   entity,
			ref CameraComponent                                 cameraComponent,
			in  CameraControlComponent                                   cameraControlComponent,
			in  DynamicBuffer<CameraIgnoredEntityBufferElement> ignoredEntitiesBuffer)
		{
			if (!CameraUtilities.TryGetCameraTargetInterpolatedWorldTransform(
				     in cameraControlComponent.FollowedCharacterEntity,
				     ref LocalToWorldLookup,
				     ref CameraTargetLookup,
				     out LocalToWorld targetWorldTransform)) return;
			float3     worldTransformUp = targetWorldTransform.Up;
			CameraUtilities.CalculateCameraRotation(ref worldTransformUp,
			                                             ref cameraComponent.PlanarForward,
			                                             cameraComponent.PitchAngle, out quaternion cameraRotation);

			float3 cameraForward  = math.mul(cameraRotation, math.forward());
			float3 targetPosition = targetWorldTransform.Position;

			// Distance smoothing
			cameraComponent.SmoothedTargetDistance = math.lerp(cameraComponent.SmoothedTargetDistance,
			                                                        cameraComponent.TargetDistance,
			                                                        MathUtilities
				                                                        .GetSharpnessInterpolant(cameraComponent
						                                                         .DistanceMovementSharpness,
					                                                         DeltaTime));

			// Obstruction handling
			// Obstruction detection is handled here, because we have to adjust the obstruction distance
			// to match the interpolated physics body transform (as opposed to the "simulation" transform). Otherwise, a
			// camera getting obstructed by a moving physics body would have visible jitter.
			if (cameraComponent.ObstructionRadius > 0f)
			{
				var obstructionCheckDistance = cameraComponent.SmoothedTargetDistance;

				var collector =
					new CameraObstructionHitsCollector(cameraControlComponent.FollowedCharacterEntity, ignoredEntitiesBuffer,
					                                   cameraForward);
				PhysicsWorld.SphereCastCustom(
				                              targetPosition,
				                              cameraComponent.ObstructionRadius,
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
					if (cameraComponent.PreventFixedUpdateJitter)
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

							collector = new CameraObstructionHitsCollector(cameraControlComponent.FollowedCharacterEntity,
							                                               ignoredEntitiesBuffer, cameraForward);
							hitBody.SphereCastCustom(
							                         targetPosition,
							                         cameraComponent.ObstructionRadius,
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
				if (cameraComponent.ObstructedDistance < newObstructedDistance)
					// Move outer
					cameraComponent.ObstructedDistance = math.lerp(cameraComponent.ObstructedDistance,
					                                                    newObstructedDistance,
					                                                    MathUtilities
						                                                    .GetSharpnessInterpolant(cameraComponent
								                                                     .ObstructionOuterSmoothingSharpness,
							                                                     DeltaTime));
				else if (cameraComponent.ObstructedDistance > newObstructedDistance)
					// Move inner
					cameraComponent.ObstructedDistance = math.lerp(cameraComponent.ObstructedDistance,
					                                                    newObstructedDistance,
					                                                    MathUtilities
						                                                    .GetSharpnessInterpolant(cameraComponent
								                                                     .ObstructionInnerSmoothingSharpness,
							                                                     DeltaTime));
			}
			else
			{
				cameraComponent.ObstructedDistance = cameraComponent.SmoothedTargetDistance;
			}

			// Place camera at the final distance (includes smoothing and obstructions)
			CameraUtilities.CalculateCameraPosition(ref targetPosition, ref cameraRotation,
			                                             cameraComponent.ObstructedDistance, out float3 cameraPosition);

			// Write to LtW
			LocalToWorldLookup[entity] = new LocalToWorld { Value = new float4x4(cameraRotation, cameraPosition) };
		}
	}
}