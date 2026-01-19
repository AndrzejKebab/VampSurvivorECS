using System;
using Unity.Entities;

namespace AndrzejKebab.Components;

[Serializable]
public struct CameraTarget : IComponentData
{
    public Entity TargetEntity;
}