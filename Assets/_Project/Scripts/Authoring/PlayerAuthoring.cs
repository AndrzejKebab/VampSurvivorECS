using AndrzejKebab.Components;
using Unity.Entities;
using UnityEngine;

namespace AndrzejKebab.Authoring;

[DisallowMultipleComponent]
public class PlayerAuthoring : MonoBehaviour
{
    public GameObject ControlledCharacter;
    public GameObject ControlledCamera;

    public class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new PlayerComponent
                                 {
                                     ControlledCharacter = GetEntity(authoring.ControlledCharacter, TransformUsageFlags.Dynamic),
                                     ControlledCamera    = GetEntity(authoring.ControlledCamera, TransformUsageFlags.Dynamic),
                                 });
            AddComponent<PlayerInputsComponent>(entity);
        }
    }
}