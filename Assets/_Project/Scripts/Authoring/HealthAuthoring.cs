using AndrzejKebab.Components;
using Unity.Entities;
using UnityEngine;

namespace AndrzejKebab.Authoring;

[DisallowMultipleComponent]
public class HealthAuthoring : MonoBehaviour
{
	public int MaxHealth = 100;

	public class Baker : Baker<HealthAuthoring>
	{
		public override void Bake(HealthAuthoring authoring)
		{
			Entity ent = GetEntity(TransformUsageFlags.None);

			AddComponent(ent, new HealthComponent
			                  {
				                  MaxHealth     = authoring.MaxHealth,
				                  CurrentHealth = authoring.MaxHealth
			                  });
		}
	}
}