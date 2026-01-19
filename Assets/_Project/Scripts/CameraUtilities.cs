using AndrzejKebab.Components;
using Unity.Burst;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AndrzejKebab;

[BurstCompile]
public static class CameraUtilities
{
	[BurstCompile]
	public static bool TryGetCameraTargetSimulationWorldTransform(
		in Entity                                   targetCharacterEntity,
		ref ComponentLookup<LocalTransform>      localTransformLookup,
		ref ComponentLookup<Parent>              parentLookup,
		ref ComponentLookup<PostTransformMatrix> postTransformMatrixLookup,
		ref ComponentLookup<CameraTargetComponent>        cameraTargetLookup,
		out float4x4                             worldTransform)
	{
		var foundValidCameraTarget = false;
		worldTransform = float4x4.identity;

		if (cameraTargetLookup.TryGetComponent(targetCharacterEntity, out CameraTargetComponent cameraTarget) &&
		    localTransformLookup.HasComponent(cameraTarget.TargetEntity))
		{
			TransformHelpers.ComputeWorldTransformMatrix(
			                                             cameraTarget.TargetEntity,
			                                             out worldTransform,
			                                             ref localTransformLookup,
			                                             ref parentLookup,
			                                             ref postTransformMatrixLookup);
			foundValidCameraTarget = true;
		}
		else if (localTransformLookup.TryGetComponent(targetCharacterEntity,
		                                              out LocalTransform characterLocalTransform))
		{
			worldTransform         = float4x4.TRS(characterLocalTransform.Position, characterLocalTransform.Rotation, 1f);
			foundValidCameraTarget = true;
		}

		return foundValidCameraTarget;
	}

	[BurstCompile]
	public static bool TryGetCameraTargetInterpolatedWorldTransform(
		in Entity                            targetCharacterEntity,
		ref ComponentLookup<LocalToWorld> localToWorldLookup,
		ref ComponentLookup<CameraTargetComponent> cameraTargetLookup,
		out LocalToWorld                  worldTransform)
	{
		var foundValidCameraTarget = false;
		worldTransform = default;

		// Get the interpolated transform of the target
		if (cameraTargetLookup.TryGetComponent(targetCharacterEntity, out CameraTargetComponent cameraTarget) &&
		    localToWorldLookup.TryGetComponent(cameraTarget.TargetEntity, out worldTransform))
			foundValidCameraTarget = true;
		else if (localToWorldLookup.TryGetComponent(targetCharacterEntity, out worldTransform))
			foundValidCameraTarget = true;

		return foundValidCameraTarget;
	}

	[BurstCompile]
	public static void CalculateCameraRotation(ref float3 targetUp, ref float3 planarForward, float pitchAngle, out quaternion cameraRotation)
	{
		quaternion pitchRotation  = quaternion.Euler(math.right() * math.radians(pitchAngle));
		cameraRotation = MathUtilities.CreateRotationWithUpPriority(targetUp, planarForward);
		cameraRotation = math.mul(cameraRotation, pitchRotation);
	}

	[BurstCompile]
	public static void CalculateCameraPosition(ref float3 targetPosition, ref quaternion cameraRotation, float distance, out float3 position)
	{
		position = targetPosition + -MathUtilities.GetForwardFromRotation(cameraRotation) * distance;
	}
}