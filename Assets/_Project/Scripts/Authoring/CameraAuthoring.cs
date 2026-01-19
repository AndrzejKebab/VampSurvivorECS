using System.Collections.Generic;
using AndrzejKebab.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AndrzejKebab.Authoring;

[DisallowMultipleComponent]
public class CameraAuthoring : MonoBehaviour
{
    [Header("Rotation")]
    public float RotationSpeed = 2f;
    public float MaxVAngle                 = 89f;
    public float MinVAngle                 = -89f;
    public bool  RotateWithCharacterParent = true;

    [Header("Distance")]
    public float StartDistance = 5f;
    public float MinDistance               = 0f;
    public float MaxDistance               = 10f;
    public float DistanceMovementSpeed     = 1f;
    public float DistanceMovementSharpness = 20f;

    [Header("Obstructions")]
    public float ObstructionRadius = 0.1f;
    public float ObstructionInnerSmoothingSharpness = float.MaxValue;
    public float ObstructionOuterSmoothingSharpness = 5f;
    public bool  PreventFixedUpdateJitter           = true;

    [Header("Misc")]
    public List<GameObject> IgnoredEntities = new ();

    public class Baker : Baker<CameraAuthoring>
    {
        public override void Bake(CameraAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.WorldSpace);

            AddComponent(entity, new CameraComponent
                                 {
                                     RotationSpeed             = authoring.RotationSpeed,
                                     MaxVAngle                 = authoring.MaxVAngle,
                                     MinVAngle                 = authoring.MinVAngle,
                                     RotateWithCharacterParent = authoring.RotateWithCharacterParent,

                                     MinDistance               = authoring.MinDistance,
                                     MaxDistance               = authoring.MaxDistance,
                                     DistanceMovementSpeed     = authoring.DistanceMovementSpeed,
                                     DistanceMovementSharpness = authoring.DistanceMovementSharpness,

                                     ObstructionRadius                  = authoring.ObstructionRadius,
                                     ObstructionInnerSmoothingSharpness = authoring.ObstructionInnerSmoothingSharpness,
                                     ObstructionOuterSmoothingSharpness = authoring.ObstructionOuterSmoothingSharpness,
                                     PreventFixedUpdateJitter           = authoring.PreventFixedUpdateJitter,

                                     TargetDistance         = authoring.StartDistance,
                                     SmoothedTargetDistance = authoring.StartDistance,
                                     ObstructedDistance     = authoring.StartDistance,

                                     PitchAngle    = 0f,
                                     PlanarForward = -math.forward(),
                                 });

            AddComponent(entity, new CameraControlComponent());

            DynamicBuffer<CameraIgnoredEntityBufferElement> ignoredEntitiesBuffer = AddBuffer<CameraIgnoredEntityBufferElement>(entity);
            foreach (GameObject t in authoring.IgnoredEntities)
            {
                ignoredEntitiesBuffer.Add(new CameraIgnoredEntityBufferElement
                                          {
                                              Entity = GetEntity(t, TransformUsageFlags.None),
                                          });
            }
        }
    }
}