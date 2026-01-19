using System;
using Unity.Entities;
using Unity.Mathematics;

namespace AndrzejKebab.Components;

[Serializable]
public struct OrbitCameraComponent : IComponentData
{
	public float RotationSpeed;
	public float MaxVAngle;
	public float MinVAngle;
	public bool  RotateWithCharacterParent;

	public float MinDistance;
	public float MaxDistance;
	public float DistanceMovementSpeed;
	public float DistanceMovementSharpness;

	public float ObstructionRadius;
	public float ObstructionInnerSmoothingSharpness;
	public float ObstructionOuterSmoothingSharpness;
	public bool  PreventFixedUpdateJitter;

	public float  TargetDistance;
	public float  SmoothedTargetDistance;
	public float  ObstructedDistance;
	public float  PitchAngle;
	public float3 PlanarForward;
}