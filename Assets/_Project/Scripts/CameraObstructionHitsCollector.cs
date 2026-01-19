using Unity.CharacterController;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace AndrzejKebab;

public struct CameraObstructionHitsCollector : ICollector<ColliderCastHit>
{
	public bool  EarlyOutOnFirstHit => false;
	public float MaxFraction        => 1f;
	public int   NumHits            { get; private set; } = 0;

	public ColliderCastHit ClosestHit = default;

	private          float                                                closestHitFraction = float.MaxValue;
	private readonly Entity                                               followedCharacter;
	private          DynamicBuffer<CameraIgnoredEntityBufferElement> ignoredEntitiesBuffer;
	private readonly float3                                               cameraDirection;

	public CameraObstructionHitsCollector(Entity                                               followedCharacter,
	                                      DynamicBuffer<CameraIgnoredEntityBufferElement> ignoredEntitiesBuffer,
	                                      float3                                               cameraDirection)
	{
		this.followedCharacter     = followedCharacter;
		this.ignoredEntitiesBuffer = ignoredEntitiesBuffer;
		this.cameraDirection       = cameraDirection;
	}

	public bool AddHit(ColliderCastHit hit)
	{
		if (followedCharacter == hit.Entity) return false;

		if (math.dot(hit.SurfaceNormal, cameraDirection) < 0f ||
		    !PhysicsUtilities.IsCollidable(hit.Material)) return false;

		for (var i = 0; i < ignoredEntitiesBuffer.Length; i++)
			if (ignoredEntitiesBuffer[i].Entity == hit.Entity)
				return false;

		if (hit.Fraction < closestHitFraction)
		{
			closestHitFraction = hit.Fraction;
			ClosestHit           = hit;
		}

		NumHits++;

		return true;
	}
}