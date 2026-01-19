using System;
using Unity.Entities;

namespace AndrzejKebab.Components;

[Serializable]
public struct PlayerComponent : IComponentData
{
    public Entity ControlledCharacter;
    public Entity ControlledCamera;
}