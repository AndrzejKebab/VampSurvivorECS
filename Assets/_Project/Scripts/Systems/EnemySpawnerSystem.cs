using AndrzejKebab.Components;
using AndrzejKebab.Components.Tags;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace AndrzejKebab.Systems
{
    [BurstCompile]
    public partial struct EnemySpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<PlayerTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<PlayerTag>()) return;

            Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
            
            if (SystemAPI.HasComponent<IsDeadTag>(playerEntity)) return;
            
            float3 playerPos    = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer.ParallelWriter ecb          = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            var job = new Jobs.SpawnEnemyJob
                      {
                          DeltaTime = SystemAPI.Time.DeltaTime,
                          PlayerPos = playerPos,
                          Ecb       = ecb
                      };

            state.Dependency = job.ScheduleParallelByRef(state.Dependency);
        }
    }
}