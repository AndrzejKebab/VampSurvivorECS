using AndrzejKebab.Components;
using Unity.Entities;
using UnityEngine;

namespace AndrzejKebab.Authoring;

public class AIControllerAuthoring : MonoBehaviour
{
	public AIControllerComponent AIControllerComponent;

	public class Baker : Baker<AIControllerAuthoring>
	{
		public override void Bake(AIControllerAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, new AIControllerComponent
			                     {
				                     DetectionDistance = authoring.AIControllerComponent.DetectionDistance,
				                     DetectionFilter   = authoring.AIControllerComponent.DetectionFilter
			                     });
			AddComponent(entity, new TargetComponent());
		}
	}
}