using System;
using Unity.Entities;

namespace AndrzejKebab.Components;

[Serializable]
public struct CameraTargetComponent : IComponentData
{
    public Entity TargetEntity;
}