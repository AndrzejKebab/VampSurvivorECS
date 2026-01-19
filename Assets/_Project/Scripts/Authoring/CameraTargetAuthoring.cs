using AndrzejKebab.Components;
using Unity.Entities;
using UnityEngine;

namespace AndrzejKebab.Authoring;

[DisallowMultipleComponent]
public class CameraTargetAuthoring : MonoBehaviour
{
    public GameObject Target;

    public class Baker : Baker<CameraTargetAuthoring>
    {
        public override void Bake(CameraTargetAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CameraTargetComponent
                                 {
                                     TargetEntity = GetEntity(authoring.Target, TransformUsageFlags.Dynamic),
                                 });
        }
    }
}