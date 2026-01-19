using System;
using Unity.Entities;
using Unity.Mathematics;

namespace AndrzejKebab.Components;

[Serializable]
public struct CharacterControllerComponent : IComponentData
{
	public float3 MoveDirection;
	public bool   IsJumping;
}