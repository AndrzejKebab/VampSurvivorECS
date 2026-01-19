using Unity.Entities;
using Unity.Mathematics;

namespace AndrzejKebab.Components;

public struct EnemySpawnerComponent : IComponentData
{
	public Entity EnemyPrefab;
	public int    MinSpawnRadius;
	public int    MaxSpawnRadius;
	public float  SpawnInterval;
	public float  TimeSinceLastSpawn;
	public Random Random;
}