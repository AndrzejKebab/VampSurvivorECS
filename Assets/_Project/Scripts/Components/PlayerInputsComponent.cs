using System;
using Unity.Entities;
using Unity.Mathematics;

namespace AndrzejKebab.Components;

[Serializable]
public struct PlayerInputsComponent : IComponentData
{
	public	float2 MoveInput;
	public float2 CameraLookInput;
	public float CameraZoomInput;
	public bool   IsJumping;
}