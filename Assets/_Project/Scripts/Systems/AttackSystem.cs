using AndrzejKebab.Components;
using AndrzejKebab.Components.Tags;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace AndrzejKebab.Systems
{
    [BurstCompile]
    public partial struct AttackSystem : ISystem
    {
        private ComponentLookup<HealthComponent> healthLookup;
        private ComponentLookup<LocalTransform>  transformLookup;
        private ComponentLookup<PlayerTag>       playerTagLookup;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            healthLookup     = state.GetComponentLookup<HealthComponent>(isReadOnly: false);
            transformLookup  = state.GetComponentLookup<LocalTransform>(isReadOnly: true);
            playerTagLookup = state.GetComponentLookup<PlayerTag>(isReadOnly: true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            healthLookup.Update(ref state);
            transformLookup.Update(ref state);
            playerTagLookup.Update(ref state);

            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb          = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            var job = new Jobs.AttackJob
                      {
                          DeltaTime       = SystemAPI.Time.DeltaTime,
                          HealthLookup    = healthLookup,
                          TransformLookup = transformLookup,
                          PlayerTagLookup = playerTagLookup,
                          Ecb             = ecb
                      };

            state.Dependency = job.ScheduleByRef(state.Dependency);
        }
    }
}
