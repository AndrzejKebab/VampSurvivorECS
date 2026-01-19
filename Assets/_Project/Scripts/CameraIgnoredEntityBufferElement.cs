using System;
using Unity.Entities;

namespace AndrzejKebab;

[Serializable]
public struct CameraIgnoredEntityBufferElement : IBufferElementData
{
    public Entity Entity;
}