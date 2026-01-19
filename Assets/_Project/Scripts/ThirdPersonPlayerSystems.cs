using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;

namespace AndrzejKebab
{
	
	[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
	[UpdateBefore(typeof(FixedStepSimulationSystemGroup))]
	public partial class ThirdPersonPlayerInputsSystem : SystemBase
	{
		protected override void OnCreate()
		{
			RequireForUpdate<FixedTickSystem.Singleton>();
			RequireForUpdate(SystemAPI.QueryBuilder().WithAll<ThirdPersonPlayer, ThirdPersonPlayerInputs>().Build());
		}

		protected override void OnUpdate()
		{
			var tick = SystemAPI.GetSingleton<FixedTickSystem.Singleton>().Tick;

			foreach ((RefRW<ThirdPersonPlayerInputs> playerInputs, RefRO<ThirdPersonPlayer> _) in SystemAPI
				         .Query<RefRW<ThirdPersonPlayerInputs>, RefRO<ThirdPersonPlayer>>())
			{
				playerInputs.ValueRW.MoveInput = new float2
				                                 {
					                                 x = (Keyboard.current.dKey.isPressed ? 1f : 0f) +
					                                     (Keyboard.current.aKey.isPressed ? -1f : 0f),
					                                 y = (Keyboard.current.wKey.isPressed ? 1f : 0f) +
					                                     (Keyboard.current.sKey.isPressed ? -1f : 0f)
				                                 };

				playerInputs.ValueRW.CameraLookInput = Mouse.current.delta.ReadValue();
				playerInputs.ValueRW.CameraZoomInput = -Mouse.current.scroll.ReadValue().y;

				if (Keyboard.current.spaceKey.wasPressedThisFrame) playerInputs.ValueRW.JumpPressed.Set(tick);
			}
		}
	}
}