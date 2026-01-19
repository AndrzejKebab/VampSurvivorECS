using System;
using Unity.Entities;
using Unity.Mathematics;

namespace AndrzejKebab.Components;

[Serializable]
public struct CharacterControlComponent : IComponentData
{
	public float3 MoveVector;
	public bool   Jump;
}