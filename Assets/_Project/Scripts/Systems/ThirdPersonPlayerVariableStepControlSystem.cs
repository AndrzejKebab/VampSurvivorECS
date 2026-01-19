using AndrzejKebab.Components;
using Unity.Burst;
using Unity.Entities;

namespace AndrzejKebab.Systems
{
	/// <summary>
	///     Apply inputs that need to be read at a variable rate
	/// </summary>
	[UpdateInGroup(typeof(SimulationSystemGroup))]
	[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
	[BurstCompile]
	public partial struct ThirdPersonPlayerVariableStepControlSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<ThirdPersonPlayer, ThirdPersonPlayerInputs>().Build());
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			foreach ((RefRO<ThirdPersonPlayerInputs> playerInputs, RefRO<ThirdPersonPlayer> player) in SystemAPI
				         .Query<RefRO<ThirdPersonPlayerInputs>, RefRO<ThirdPersonPlayer>>().WithAll<Simulate>())
				if (SystemAPI.HasComponent<OrbitCameraControl>(player.ValueRO.ControlledCamera))
				{
					var cameraControl = SystemAPI.GetComponent<OrbitCameraControl>(player.ValueRO.ControlledCamera);

					cameraControl.FollowedCharacterEntity = player.ValueRO.ControlledCharacter;
					cameraControl.LookDegreesDelta        = playerInputs.ValueRO.CameraLookInput;
					cameraControl.ZoomDelta               = playerInputs.ValueRO.CameraZoomInput;

					SystemAPI.SetComponent(player.ValueRO.ControlledCamera, cameraControl);
				}
		}
	}
}