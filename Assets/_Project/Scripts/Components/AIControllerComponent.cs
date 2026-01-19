using System;
using Unity.Entities;
using Unity.Physics.Authoring;

namespace AndrzejKebab.Components;

[Serializable]
public struct AIControllerComponent : IComponentData
{
	public float               DetectionDistance;
	public PhysicsCategoryTags DetectionFilter;
}