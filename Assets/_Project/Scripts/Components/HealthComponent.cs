using Unity.Entities;

namespace AndrzejKebab.Components;

public struct HealthComponent : IComponentData
{
	public int MaxHealth;
	public int CurrentHealth;
}