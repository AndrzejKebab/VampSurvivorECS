using System;
using Unity.Entities;

namespace AndrzejKebab.Components;

[Serializable]
public struct ThirdPersonPlayer : IComponentData
{
    public Entity ControlledCharacter;
    public Entity ControlledCamera;
}