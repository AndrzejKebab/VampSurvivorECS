using AndrzejKebab.Components;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace AndrzejKebab.Authoring;

[DisallowMultipleComponent]
public class CharacterAuthoring : MonoBehaviour
{
    public AuthoringKinematicCharacterProperties CharacterProperties = AuthoringKinematicCharacterProperties.GetDefault();

    public float                               RotationSharpness                           = 25f;
    public float                               GroundMaxSpeed                              = 10f;
    public float                               GroundedMovementSharpness                   = 15f;
    public float                               AirAcceleration                             = 50f;
    public float                               AirMaxSpeed                                 = 10f;
    public float                               AirDrag                                     = 0f;
    public float                               JumpSpeed                                   = 10f;
    public float3                              Gravity                                     = math.up() * -30f;
    public bool                                PreventAirAccelerationAgainstUngroundedHits = true;
    public BasicStepAndSlopeHandlingParameters StepAndSlopeHandling                        = BasicStepAndSlopeHandlingParameters.GetDefault();

    public class Baker : Baker<CharacterAuthoring>
    {
        public override void Bake(CharacterAuthoring authoring)
        {
            KinematicCharacterUtilities.BakeCharacter(this, authoring.gameObject, authoring.CharacterProperties);

            Entity entity = GetEntity(TransformUsageFlags.Dynamic | TransformUsageFlags.WorldSpace);

            AddComponent(entity, new CharacterComponent
                                 {
                                     RotationSharpness                           = authoring.RotationSharpness,
                                     GroundMaxSpeed                              = authoring.GroundMaxSpeed,
                                     GroundedMovementSharpness                   = authoring.GroundedMovementSharpness,
                                     AirAcceleration                             = authoring.AirAcceleration,
                                     AirMaxSpeed                                 = authoring.AirMaxSpeed,
                                     AirDrag                                     = authoring.AirDrag,
                                     JumpSpeed                                   = authoring.JumpSpeed,
                                     Gravity                                     = authoring.Gravity,
                                     PreventAirAccelerationAgainstUngroundedHits = authoring.PreventAirAccelerationAgainstUngroundedHits,
                                     StepAndSlopeHandling                        = authoring.StepAndSlopeHandling,
                                 });
            AddComponent(entity, new CharacterControlComponent());
        }
    }

}