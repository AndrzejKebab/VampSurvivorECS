using AndrzejKebab.Components;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace AndrzejKebab.Authoring;

[DisallowMultipleComponent]
public class EnemySpawnerAuthoring : MonoBehaviour
{
	public GameObject EnemyPrefab;
	public int        MinSpawnRadius = 5;
	public int        MaxSpawnRadius = 100;
	public float      SpawnInterval  = 0.05f;

	public class EnemySpawnerAuthoringBaker : Baker<EnemySpawnerAuthoring>
	{
		public override void Bake(EnemySpawnerAuthoring authoring)
		{
			Entity entity            = GetEntity(TransformUsageFlags.None);
			Entity enemyEntityPrefab = GetEntity(authoring.EnemyPrefab, TransformUsageFlags.Dynamic);

			AddComponent(entity, new EnemySpawnerComponent
			                     {
				                     EnemyPrefab        = enemyEntityPrefab,
				                     SpawnInterval      = authoring.SpawnInterval,
				                     MinSpawnRadius     = authoring.MinSpawnRadius,
				                     MaxSpawnRadius     = authoring.MaxSpawnRadius,
				                     TimeSinceLastSpawn = 0,
				                     Random             = new Random(2137)
			                     });
		}
	}
}