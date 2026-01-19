using AndrzejKebab.Components;
using Unity.Entities;
using UnityEngine;

namespace AndrzejKebab.Authoring;

public class PlayerTagAuthoring : MonoBehaviour
{
	public class PlayerTagAuthoringBaker : Baker<PlayerTagAuthoring>
	{
		public override void Bake(PlayerTagAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent<LinkedEntityGroup>(entity);
			AddComponent(entity, new PlayerTag());
			AddComponent(entity, new TargetComponent());
		}
	}
}