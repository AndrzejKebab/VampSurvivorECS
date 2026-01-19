using AndrzejKebab.Components;
using AndrzejKebab.Components.Tags;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace AndrzejKebab.Systems
{
    [BurstCompile]
    public partial struct AIControllerSystem : ISystem
    {
        private ComponentLookup<CharacterComponent> characterLookup;
        private ComponentLookup<AIControllerComponent>         aiLookup;
        private ComponentLookup<LocalTransform>                transformLookup;
        private ComponentLookup<IsDeadTag>                     deadLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            characterLookup = state.GetComponentLookup<CharacterComponent>(true);
            aiLookup        = state.GetComponentLookup<AIControllerComponent>(true);
            transformLookup = state.GetComponentLookup<LocalTransform>(true);
            deadLookup      = state.GetComponentLookup<IsDeadTag>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            characterLookup.Update(ref state);
            aiLookup.Update(ref state);
            transformLookup.Update(ref state);
            deadLookup.Update(ref state);

            PhysicsWorld physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

            var job = new Jobs.AIControlJob
                      {
                          PhysicsWorld    = physicsWorld,
                          CharacterLookup = characterLookup,
                          AILookup        = aiLookup,
                          TransformLookup = transformLookup,
                          DeadLookup      = deadLookup
                      };

            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
    }
}