using AndrzejKebab.Components;
using Unity.Entities;
using UnityEngine;

namespace AndrzejKebab.Authoring;

[DisallowMultipleComponent]
public class MainCameraEntityAuthoring : MonoBehaviour
{
    public class Baker : Baker<MainCameraEntityAuthoring>
    {
        public override void Bake(MainCameraEntityAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<MainCameraEntityTag>(entity);
        }
    }
}