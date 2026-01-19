using AndrzejKebab.Components;
using AndrzejKebab.Components.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace AndrzejKebab.Systems
{
    [BurstCompile]
    public partial struct PlayerTargetingSystem : ISystem
    {
        private EntityQuery enemyQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerTag>();
            
            enemyQuery = SystemAPI.QueryBuilder()
                .WithAll<EnemyTag, LocalTransform>()
                .Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (!SystemAPI.HasSingleton<PlayerTag>()) return;

            Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
            float3 playerPos = SystemAPI.GetComponent<LocalTransform>(playerEntity).Position;

            NativeArray<Entity> enemyEntities = enemyQuery.ToEntityArray(Allocator.TempJob);
            NativeArray<LocalTransform> enemyTransforms = enemyQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);

            var job = new FindClosestEnemyJob
            {
                PlayerPos = playerPos,
                EnemyEntities = enemyEntities,
                EnemyTransforms = enemyTransforms,
                ResultTarget = new NativeReference<Entity>(Allocator.TempJob) 
            };

            state.Dependency = job.ScheduleByRef(state.Dependency);
            
            state.Dependency.Complete();

            if (job.ResultTarget.Value != Entity.Null)
            {
                SystemAPI.GetComponentRW<TargetComponent>(playerEntity).ValueRW.TargetEntity = job.ResultTarget.Value;
            }

            enemyEntities.Dispose();
            enemyTransforms.Dispose();
            job.ResultTarget.Dispose();
        }
    }
}