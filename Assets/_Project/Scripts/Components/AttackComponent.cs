using Unity.Entities;

namespace AndrzejKebab.Components;

public struct AttackComponent : IComponentData
{
	public int   AttackDamage;
	public float AttackRange;
	public float AttackSpeed;
	public float TimeSinceLastAttack;
}