using AndrzejKebab.Components;
using AndrzejKebab.Components.Tags;
using Unity.Entities;
using UnityEngine;

namespace AndrzejKebab.Authoring;

public class AIControllerAuthoring : MonoBehaviour
{
	[Header("AI Settings")]
	public AIControllerComponent AIControllerComponent;

	public class Baker : Baker<AIControllerAuthoring>
	{
		public override void Bake(AIControllerAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);

			AddComponent(entity, authoring.AIControllerComponent);
			AddComponent(entity, new TargetComponent());
		}
	}
}