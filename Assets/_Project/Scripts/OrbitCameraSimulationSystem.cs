using Unity.Burst;
using Unity.CharacterController;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AndrzejKebab
{
	[UpdateInGroup(typeof(SimulationSystemGroup))]
	[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
	[UpdateAfter(typeof(ThirdPersonPlayerVariableStepControlSystem))]
	[UpdateAfter(typeof(ThirdPersonCharacterVariableUpdateSystem))]
	[UpdateBefore(typeof(TransformSystemGroup))]
	[BurstCompile]
	public partial struct OrbitCameraSimulationSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<OrbitCameraComponent, OrbitCameraControl>()
			                                .Build());
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			var job = new OrbitCameraSimulationJob
			          {
				          DeltaTime                    = SystemAPI.Time.DeltaTime,
				          LocalTransformLookup         = SystemAPI.GetComponentLookup<LocalTransform>(),
				          ParentLookup                 = SystemAPI.GetComponentLookup<Parent>(true),
				          PostTransformMatrixLookup    = SystemAPI.GetComponentLookup<PostTransformMatrix>(true),
				          CameraTargetLookup           = SystemAPI.GetComponentLookup<CameraTarget>(true),
				          KinematicCharacterBodyLookup = SystemAPI.GetComponentLookup<KinematicCharacterBody>(true)
			          };
			job.Schedule();
		}

		[BurstCompile]
		[WithAll(typeof(Simulate))]
		public partial struct OrbitCameraSimulationJob : IJobEntity
		{
			public float DeltaTime;

			public            ComponentLookup<LocalTransform>         LocalTransformLookup;
			[ReadOnly] public ComponentLookup<Parent>                 ParentLookup;
			[ReadOnly] public ComponentLookup<PostTransformMatrix>    PostTransformMatrixLookup;
			[ReadOnly] public ComponentLookup<CameraTarget>           CameraTargetLookup;
			[ReadOnly] public ComponentLookup<KinematicCharacterBody> KinematicCharacterBodyLookup;

			private void Execute(Entity                entity, ref OrbitCameraComponent orbitCameraComponent,
			                     in OrbitCameraControl cameraControl)
			{
				if (!OrbitCameraUtilities.TryGetCameraTargetSimulationWorldTransform(
				     cameraControl.FollowedCharacterEntity,
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
						MathUtilities.CreateRotationWithUpPriority(targetUp, orbitCameraComponent.PlanarForward);

					// Rotation from character parent
					if (orbitCameraComponent.RotateWithCharacterParent &&
					    KinematicCharacterBodyLookup.TryGetComponent(cameraControl.FollowedCharacterEntity,
					                                                 out KinematicCharacterBody characterBody))
					{
						// Only consider rotation around the character up, since the camera is already adjusting itself to character up
						quaternion planarRotationFromParent = characterBody.RotationFromParent;
						KinematicCharacterUtilities.AddVariableRateRotationFromFixedRateRotation(ref tmpPlanarRotation,
						 planarRotationFromParent, DeltaTime, characterBody.LastPhysicsUpdateDeltaTime);
					}

					orbitCameraComponent.PlanarForward = MathUtilities.GetForwardFromRotation(tmpPlanarRotation);
				}

				// Yaw
				var        yawAngleChange = cameraControl.LookDegreesDelta.x * orbitCameraComponent.RotationSpeed;
				quaternion yawRotation    = quaternion.Euler(targetUp * math.radians(yawAngleChange));
				orbitCameraComponent.PlanarForward = math.rotate(yawRotation, orbitCameraComponent.PlanarForward);

				// Pitch
				orbitCameraComponent.PitchAngle +=
					-cameraControl.LookDegreesDelta.y * orbitCameraComponent.RotationSpeed;
				orbitCameraComponent.PitchAngle = math.clamp(orbitCameraComponent.PitchAngle,
				                                             orbitCameraComponent.MinVAngle,
				                                             orbitCameraComponent.MaxVAngle);

				// Calculate final rotation
				quaternion cameraRotation =
					OrbitCameraUtilities.CalculateCameraRotation(targetUp, orbitCameraComponent.PlanarForward,
					                                             orbitCameraComponent.PitchAngle);

				// Distance input
				var desiredDistanceMovementFromInput =
					cameraControl.ZoomDelta * orbitCameraComponent.DistanceMovementSpeed;
				orbitCameraComponent.TargetDistance =
					math.clamp(orbitCameraComponent.TargetDistance + desiredDistanceMovementFromInput,
					           orbitCameraComponent.MinDistance, orbitCameraComponent.MaxDistance);

				// Calculate camera position (no smoothing or obstructions yet; these are done in the camera late update)
				float3 cameraPosition =
					OrbitCameraUtilities.CalculateCameraPosition(targetPosition, cameraRotation,
					                                             orbitCameraComponent.TargetDistance);

				// Write back to component
				LocalTransformLookup[entity] = LocalTransform.FromPositionRotation(cameraPosition, cameraRotation);
			}
		}
	}
}