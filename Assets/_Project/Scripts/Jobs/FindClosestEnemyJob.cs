using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace AndrzejKebab.Jobs
{
	[BurstCompile]
	public struct FindClosestEnemyJob : IJob
	{
		public            float3                      PlayerPos;
		[ReadOnly] public NativeArray<Entity>         EnemyEntities;
		[ReadOnly] public NativeArray<LocalTransform> EnemyTransforms;
        
		[WriteOnly] public NativeReference<Entity> ResultTarget;

		public void Execute()
		{
			var    closestDistSq = float.MaxValue;
			Entity closestEntity = Entity.Null;

			for (var i = 0; i < EnemyEntities.Length; i++)
			{
				var distSq = math.distancesq(PlayerPos, EnemyTransforms[i].Position);

				if (!(distSq < closestDistSq)) continue;
				closestDistSq = distSq;
				closestEntity = EnemyEntities[i];
			}

			ResultTarget.Value = closestEntity;
		}
	}
}