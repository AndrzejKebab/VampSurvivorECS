using System;
using Unity.Entities;
using Unity.Mathematics;

namespace AndrzejKebab;

[Serializable]
public struct ThirdPersonPlayerInputs : IComponentData
{
	public float2          MoveInput;
	public float2          CameraLookInput;
	public float           CameraZoomInput;
	public FixedInputEvent JumpPressed;
}