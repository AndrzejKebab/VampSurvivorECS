using System;
using Unity.Entities;
using Unity.Mathematics;

namespace AndrzejKebab.Components;

[Serializable]
public struct OrbitCameraControl : IComponentData
{
	public Entity FollowedCharacterEntity;
	public float2 LookDegreesDelta;
	public float  ZoomDelta;
}