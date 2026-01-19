using AndrzejKebab.Components;
using AndrzejKebab.Components.Tags;
using Unity.Burst;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AndrzejKebab.Systems
{
	[UpdateInGroup(typeof(SimulationSystemGroup))]
	[UpdateBefore(typeof(FixedStepSimulationSystemGroup))] 
	public partial struct PlayerControllerSystem : ISystem
	{
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
        foreach ((RefRO<PlayerReferencesComponent> refs, RefRO<PlayerInputsComponent> inputs) in 
                 SystemAPI.Query<RefRO<PlayerReferencesComponent>, RefRO<PlayerInputsComponent>>())
        {
            Entity characterEntity = refs.ValueRO.ControlledCharacter;
            Entity cameraEntity = refs.ValueRO.ControlledCamera;

            if (SystemAPI.HasComponent<IsDeadTag>(characterEntity)) 
            {
                if (SystemAPI.HasComponent<ThirdPersonCharacterControl>(characterEntity))
                {
                    RefRW<ThirdPersonCharacterControl> deadControl = SystemAPI.GetComponentRW<ThirdPersonCharacterControl>(characterEntity);
                    deadControl.ValueRW.MoveVector = float3.zero;
                    deadControl.ValueRW.Jump = false;
                }
                continue;
            }

            if (!SystemAPI.HasComponent<ThirdPersonCharacterControl>(characterEntity)) continue;
            if (!SystemAPI.HasComponent<LocalTransform>(characterEntity)) continue;
            if (!SystemAPI.HasComponent<OrbitCameraComponent>(cameraEntity)) continue;

            var orbitCamera = SystemAPI.GetComponent<OrbitCameraComponent>(cameraEntity);
            var charTransform = SystemAPI.GetComponent<LocalTransform>(characterEntity);
            
            float3 characterUp = MathUtilities.GetUpFromRotation(charTransform.Rotation);
            
            quaternion cameraRotation = OrbitCameraUtilities.CalculateCameraRotation(characterUp, orbitCamera.PlanarForward, orbitCamera.PitchAngle);
            
            float3 cameraForwardOnUpPlane = math.normalizesafe(MathUtilities.ProjectOnPlane(MathUtilities.GetForwardFromRotation(cameraRotation), characterUp));
            float3 cameraRight = MathUtilities.GetRightFromRotation(cameraRotation);

            float3 moveVec = (inputs.ValueRO.MoveInput.y * cameraForwardOnUpPlane) + (inputs.ValueRO.MoveInput.x * cameraRight);
            moveVec = MathUtilities.ClampToMaxLength(moveVec, 1f);

            RefRW<ThirdPersonCharacterControl> control = SystemAPI.GetComponentRW<ThirdPersonCharacterControl>(characterEntity);
            control.ValueRW.MoveVector = moveVec;
            control.ValueRW.Jump = inputs.ValueRO.IsJumping;

            if (!SystemAPI.HasComponent<OrbitCameraControl>(cameraEntity)) continue;
            RefRW<OrbitCameraControl> camControl = SystemAPI.GetComponentRW<OrbitCameraControl>(cameraEntity);
            camControl.ValueRW.LookDegreesDelta        = inputs.ValueRO.CameraLookInput;
            camControl.ValueRW.ZoomDelta               = inputs.ValueRO.CameraZoomInput;
            camControl.ValueRW.FollowedCharacterEntity = characterEntity;
        }
		}
	}
}