using Unity.Entities;

namespace AndrzejKebab.Components;

public struct PlayerReferencesComponent : IComponentData
{
	public Entity ControlledCharacter;
	public Entity ControlledCamera;
}