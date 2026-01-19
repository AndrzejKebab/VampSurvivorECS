using AndrzejKebab.Components;
using Unity.Entities;
using UnityEngine;

namespace AndrzejKebab.Authoring;

public class AttackAuthoring : MonoBehaviour
{
	public int   AttackDamage = 10;
	public float AttackRange = 1;
	public float AttackSpeed = 1;

	public class Baker : Baker<AttackAuthoring>
	{
		public override void Bake(AttackAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.None);

			AddComponent(entity, new AttackComponent
			                     {
				                     AttackDamage = authoring.AttackDamage,
				                     AttackRange  = authoring.AttackRange,
				                     AttackSpeed  = authoring.AttackSpeed,
				                     TimeSinceLastAttack = 0
			                     });
		}
	}
}