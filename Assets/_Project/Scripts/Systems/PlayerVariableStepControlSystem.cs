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
	public partial struct PlayerVariableStepControlSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<PlayerComponent, PlayerInputsComponent>().Build());
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			foreach ((RefRO<PlayerInputsComponent> playerInputs, RefRO<PlayerComponent> player) in SystemAPI
				         .Query<RefRO<PlayerInputsComponent>, RefRO<PlayerComponent>>().WithAll<Simulate>())
				if (SystemAPI.HasComponent<CameraControlComponent>(player.ValueRO.ControlledCamera))
				{
					var cameraControl = SystemAPI.GetComponent<CameraControlComponent>(player.ValueRO.ControlledCamera);

					cameraControl.FollowedCharacterEntity = player.ValueRO.ControlledCharacter;
					cameraControl.LookDegreesDelta        = playerInputs.ValueRO.CameraLookInput;
					cameraControl.ZoomDelta               = playerInputs.ValueRO.CameraZoomInput;

					SystemAPI.SetComponent(player.ValueRO.ControlledCamera, cameraControl);
				}
		}
	}
}