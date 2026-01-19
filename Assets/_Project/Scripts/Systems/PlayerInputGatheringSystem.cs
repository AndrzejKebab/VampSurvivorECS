using AndrzejKebab.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.InputSystem;

namespace AndrzejKebab.Systems
{
	[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst =  true)]
	public partial struct PlayerInputGatheringSystem : ISystem
    {
	    public void OnUpdate(ref SystemState state)
	    {
		    foreach (RefRW<PlayerInputsComponent> inputs in SystemAPI.Query<RefRW<PlayerInputsComponent>>())
		    {
#if ENABLE_INPUT_SYSTEM
			    inputs.ValueRW.MoveInput = new float2
			                               {
				                               x = (Keyboard.current.dKey.isPressed ? 1f : 0f) + (Keyboard.current.aKey.isPressed ? -1f : 0f),
				                               y = (Keyboard.current.wKey.isPressed ? 1f : 0f) + (Keyboard.current.sKey.isPressed ? -1f : 0f),
			                               };
			    inputs.ValueRW.CameraLookInput = Mouse.current.delta.ReadValue();
			    inputs.ValueRW.CameraZoomInput = -Mouse.current.scroll.ReadValue().y;
			    inputs.ValueRW.IsJumping     = Keyboard.current.spaceKey.isPressed;
#endif
		    }
	    }
    }
}