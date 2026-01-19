using System;
using Unity.Entities;

namespace AndrzejKebab;

[Serializable]
public struct OrbitCameraIgnoredEntityBufferElement : IBufferElementData
{
    public Entity Entity;
}