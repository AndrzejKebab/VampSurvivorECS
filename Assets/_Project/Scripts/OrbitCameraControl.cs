using System;
using Unity.Entities;
using Unity.Mathematics;

namespace AndrzejKebab;

[Serializable]
public struct OrbitCameraControl : IComponentData
{
	public Entity FollowedCharacterEntity;
	public float2 LookDegreesDelta;
	public float  ZoomDelta;
}