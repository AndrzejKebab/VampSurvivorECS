using AndrzejKebab.Components;
using Unity.Burst;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Physics;

namespace AndrzejKebab.Systems
{
    [UpdateInGroup(typeof(KinematicCharacterPhysicsUpdateGroup))]
    [BurstCompile]
    public partial struct CharacterPhysicsUpdateSystem : ISystem
    {
        private EntityQuery                       characterQuery;
        private CharacterUpdateContext context;
        private KinematicCharacterUpdateContext   baseContext;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            characterQuery = KinematicCharacterUtilities.GetBaseCharacterQueryBuilder()
                                                        .WithAll<CharacterComponent,
                                                            CharacterControlComponent>()
                                                        .Build(ref state);

            context = new CharacterUpdateContext();
            context.OnSystemCreate(ref state);
            baseContext = new KinematicCharacterUpdateContext();
            baseContext.OnSystemCreate(ref state);

            state.RequireForUpdate(characterQuery);
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            context.OnSystemUpdate(ref state);
            baseContext.OnSystemUpdate(ref state, SystemAPI.Time, SystemAPI.GetSingleton<PhysicsWorldSingleton>());

            var job = new Jobs.CharacterPhysicsUpdateJob
                      {
                          Context     = context,
                          BaseContext = baseContext,
                      };
            job.ScheduleParallel();
        }
    }
}