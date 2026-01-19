using AndrzejKebab.Components.Tags;
using Unity.Entities;
using UnityEngine;

namespace AndrzejKebab.Authoring;

public class EnemyTagAuthoring : MonoBehaviour, IComponentData
{
	public class Baker : Baker<EnemyTagAuthoring>
	{
		public override void Bake(EnemyTagAuthoring authoring)
		{
			Entity ent = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(ent, new EnemyTag());
		}
	}
}