using AndrzejKebab.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace AndrzejKebab.Systems
{
    [BurstCompile]
    public partial struct AIControllerSystem : ISystem
    {
        private ComponentLookup<ThirdPersonCharacterComponent> characterLookup;
        private ComponentLookup<AIControllerComponent> aiLookup;
        private ComponentLookup<LocalTransform> transformLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            
            characterLookup = state.GetComponentLookup<ThirdPersonCharacterComponent>(isReadOnly: true);
            aiLookup = state.GetComponentLookup<AIControllerComponent>(isReadOnly: true);
            transformLookup = state.GetComponentLookup<LocalTransform>(isReadOnly: true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            characterLookup.Update(ref state);
            aiLookup.Update(ref state);
            transformLookup.Update(ref state);

            PhysicsWorld physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

            var job = new AIControlJob
            {
                PhysicsWorld = physicsWorld,
                CharacterLookup = characterLookup,
                AILookup = aiLookup,
                TransformLookup = transformLookup
            };

            state.Dependency = job.ScheduleByRef(state.Dependency);
        }
    }
}