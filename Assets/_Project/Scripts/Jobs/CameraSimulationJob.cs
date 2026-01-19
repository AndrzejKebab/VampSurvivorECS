using AndrzejKebab.Components;
using Unity.Burst;
using Unity.CharacterController;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AndrzejKebab.Jobs
{
	[BurstCompile]
	[WithAll(typeof(Simulate))]
	public partial struct CameraSimulationJob : IJobEntity
	{
		public float DeltaTime;

		public            ComponentLookup<LocalTransform>         LocalTransformLookup;
		[ReadOnly] public ComponentLookup<Parent>                 ParentLookup;
		[ReadOnly] public ComponentLookup<PostTransformMatrix>    PostTransformMatrixLookup;
		[ReadOnly] public ComponentLookup<CameraTargetComponent>  CameraTargetLookup;
		[ReadOnly] public ComponentLookup<KinematicCharacterBody> KinematicCharacterBodyLookup;

		private void Execute(Entity                    entity, ref CameraComponent cameraComponent,
		                     in CameraControlComponent cameraControlComponent)
		{
			if (!CameraUtilities.TryGetCameraTargetSimulationWorldTransform(
				     in cameraControlComponent.FollowedCharacterEntity,
				     ref LocalTransformLookup,
				     ref ParentLookup,
				     ref PostTransformMatrixLookup,
				     ref CameraTargetLookup,
				     out float4x4 targetWorldTransform)) return;
			float3 targetUp       = targetWorldTransform.Up();
			float3 targetPosition = targetWorldTransform.Translation();

			// Update planar forward based on target up direction and rotation from parent
			{
				quaternion tmpPlanarRotation =
					MathUtilities.CreateRotationWithUpPriority(targetUp, cameraComponent.PlanarForward);

				// Rotation from character parent
				if (cameraComponent.RotateWithCharacterParent &&
				    KinematicCharacterBodyLookup.TryGetComponent(cameraControlComponent.FollowedCharacterEntity,
				                                                 out KinematicCharacterBody characterBody))
				{
					// Only consider rotation around the character up, since the camera is already adjusting itself to character up
					quaternion planarRotationFromParent = characterBody.RotationFromParent;
					KinematicCharacterUtilities.AddVariableRateRotationFromFixedRateRotation(ref tmpPlanarRotation,
						 planarRotationFromParent, DeltaTime, characterBody.LastPhysicsUpdateDeltaTime);
				}

				cameraComponent.PlanarForward = MathUtilities.GetForwardFromRotation(tmpPlanarRotation);
			}

			// Yaw
			var        yawAngleChange = cameraControlComponent.LookDegreesDelta.x * cameraComponent.RotationSpeed;
			quaternion yawRotation    = quaternion.Euler(targetUp * math.radians(yawAngleChange));
			cameraComponent.PlanarForward = math.rotate(yawRotation, cameraComponent.PlanarForward);

			// Pitch
			cameraComponent.PitchAngle +=
				-cameraControlComponent.LookDegreesDelta.y * cameraComponent.RotationSpeed;
			cameraComponent.PitchAngle = math.clamp(cameraComponent.PitchAngle,
			                                        cameraComponent.MinVAngle,
			                                        cameraComponent.MaxVAngle);

			// Calculate final rotation
			CameraUtilities.CalculateCameraRotation(ref targetUp, ref cameraComponent.PlanarForward,
			                                             cameraComponent.PitchAngle, out quaternion cameraRotation);

			// Distance input
			var desiredDistanceMovementFromInput =
				cameraControlComponent.ZoomDelta * cameraComponent.DistanceMovementSpeed;
			cameraComponent.TargetDistance =
				math.clamp(cameraComponent.TargetDistance + desiredDistanceMovementFromInput,
				           cameraComponent.MinDistance, cameraComponent.MaxDistance);

			// Calculate camera position (no smoothing or obstructions yet; these are done in the camera late update)
			CameraUtilities.CalculateCameraPosition(ref targetPosition, ref cameraRotation,
			                                             cameraComponent.TargetDistance, out float3 cameraPosition);

			// Write back to component
			LocalTransformLookup[entity] = LocalTransform.FromPositionRotation(cameraPosition, cameraRotation);
		}
	}
}