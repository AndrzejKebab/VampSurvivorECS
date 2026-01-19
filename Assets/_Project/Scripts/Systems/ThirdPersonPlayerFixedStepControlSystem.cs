using AndrzejKebab.Components;
using AndrzejKebab.Components.Tags;
using Unity.Burst;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AndrzejKebab.Systems
{
	/// <summary>
	///     Apply inputs that need to be read at a fixed rate.
	///     It is necessary to handle this as part of the fixed step group, in case your framerate is lower than the fixed step
	///     rate.
	/// </summary>
	[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
	[BurstCompile]
	public partial struct ThirdPersonPlayerFixedStepControlSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<Systems.FixedTickSystem.Singleton>();
			state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<ThirdPersonPlayer, ThirdPersonPlayerInputs>()
			                                .WithNone<IsDeadTag>().Build());
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			var tick = SystemAPI.GetSingleton<Systems.FixedTickSystem.Singleton>().Tick;

			foreach ((RefRO<ThirdPersonPlayerInputs> playerInputs, RefRO<ThirdPersonPlayer> player) in SystemAPI
				         .Query<RefRO<ThirdPersonPlayerInputs>, RefRO<ThirdPersonPlayer>>().WithAll<Simulate>()
				         .WithNone<IsDeadTag>())
				if (SystemAPI.HasComponent<ThirdPersonCharacterControl>(player.ValueRO.ControlledCharacter))
				{
					var characterControl =
						SystemAPI.GetComponent<ThirdPersonCharacterControl>(player.ValueRO.ControlledCharacter);

					float3 characterUp =
						MathUtilities.GetUpFromRotation(SystemAPI
						                                .GetComponent<
							                                LocalTransform>(player.ValueRO.ControlledCharacter)
						                                .Rotation);

					// Get camera rotation, since our movement is relative to it.
					quaternion cameraRotation = quaternion.identity;
					if (SystemAPI.HasComponent<OrbitCameraComponent>(player.ValueRO.ControlledCamera))
					{
						// Camera rotation is calculated rather than gotten from transform, because this allows us to
						// reduce the size of the camera ghost state in a netcode prediction context.
						// If not using netcode prediction, we could simply get rotation from transform here instead.
						var orbitCamera = SystemAPI.GetComponent<OrbitCameraComponent>(player.ValueRO.ControlledCamera);
						OrbitCameraUtilities.CalculateCameraRotation(ref characterUp, ref orbitCamera.PlanarForward,
							                                             orbitCamera.PitchAngle, out cameraRotation);
					}

					float3 cameraForwardOnUpPlane =
						math.normalizesafe(MathUtilities.ProjectOnPlane(MathUtilities
							                                                .GetForwardFromRotation(cameraRotation),
						                                                characterUp));
					float3 cameraRight = MathUtilities.GetRightFromRotation(cameraRotation);

					// Move
					characterControl.MoveVector = playerInputs.ValueRO.MoveInput.y * cameraForwardOnUpPlane +
					                              playerInputs.ValueRO.MoveInput.x * cameraRight;
					characterControl.MoveVector = MathUtilities.ClampToMaxLength(characterControl.MoveVector, 1f);

					// Jump
					characterControl.Jump = playerInputs.ValueRO.JumpPressed.IsSet(tick);

					SystemAPI.SetComponent(player.ValueRO.ControlledCharacter, characterControl);
				}
		}
	}
}