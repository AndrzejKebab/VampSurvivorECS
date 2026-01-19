using AndrzejKebab.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AndrzejKebab.Jobs
{
	[BurstCompile]
	public partial struct SpawnEnemyJob : IJobEntity
	{
		[ReadOnly]  public float  DeltaTime;
		[ReadOnly]  public Entity CameraEntity;
		[WriteOnly] public float3 PlayerPos;
		[WriteOnly] public EntityCommandBuffer.ParallelWriter Ecb;

		private void Execute([EntityIndexInQuery] int sortKey, ref EnemySpawnerComponent spawner)
		{
			spawner.TimeSinceLastSpawn += DeltaTime;

			if (!(spawner.TimeSinceLastSpawn >= spawner.SpawnInterval)) return;
			spawner.TimeSinceLastSpawn = 0f;

			Entity newEnemy = Ecb.Instantiate(sortKey, spawner.EnemyPrefab);

			var angle    = spawner.Random.NextFloat(0, math.PI * 2);
			var distance = spawner.Random.NextFloat(spawner.MinSpawnRadius, spawner.MaxSpawnRadius);

			var x = math.cos(angle) * distance;
			var z = math.sin(angle) * distance;

			float3 spawnPos = PlayerPos + new float3(x, 0, z);

			Ecb.SetComponent(sortKey, newEnemy, LocalTransform.FromPosition(spawnPos));
			
			Ecb.AppendToBuffer(sortKey, CameraEntity, new CameraIgnoredEntityBufferElement
			                                          {
				                                          Entity = newEnemy
			                                          });
		}
	}	
}