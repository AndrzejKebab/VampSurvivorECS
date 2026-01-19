using System;
using Unity.Entities;

namespace AndrzejKebab;

[Serializable]
public struct CameraTarget : IComponentData
{
    public Entity TargetEntity;
}