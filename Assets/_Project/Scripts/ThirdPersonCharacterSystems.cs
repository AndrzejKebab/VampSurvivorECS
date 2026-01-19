using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace AndrzejKebab
{

    [UpdateInGroup(typeof(KinematicCharacterPhysicsUpdateGroup))]
    [BurstCompile]
    public partial struct ThirdPersonCharacterPhysicsUpdateSystem : ISystem
    {
        private EntityQuery                       characterQuery;
        private ThirdPersonCharacterUpdateContext context;
        private KinematicCharacterUpdateContext   baseContext;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            characterQuery = KinematicCharacterUtilities.GetBaseCharacterQueryBuilder()
                                                        .WithAll<ThirdPersonCharacterComponent,
                                                            ThirdPersonCharacterControl>()
                                                        .Build(ref state);

            context = new ThirdPersonCharacterUpdateContext();
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

            var job = new ThirdPersonCharacterPhysicsUpdateJob
                      {
                          Context     = context,
                          BaseContext = baseContext,
                      };
            job.ScheduleParallel();
        }

        [BurstCompile]
        [WithAll(typeof(Simulate))]
        public partial struct ThirdPersonCharacterPhysicsUpdateJob : IJobEntity, IJobEntityChunkBeginEnd
        {
            public ThirdPersonCharacterUpdateContext Context;
            public KinematicCharacterUpdateContext   BaseContext;

            private void Execute(
                Entity                                           entity,
                RefRW<LocalTransform>                            localTransform,
                RefRW<KinematicCharacterProperties>              characterProperties,
                RefRW<KinematicCharacterBody>                    characterBody,
                RefRW<PhysicsCollider>                           physicsCollider,
                RefRW<ThirdPersonCharacterComponent>             characterComponent,
                RefRW<ThirdPersonCharacterControl>               characterControl,
                DynamicBuffer<KinematicCharacterHit>             characterHitsBuffer,
                DynamicBuffer<StatefulKinematicCharacterHit>     statefulHitsBuffer,
                DynamicBuffer<KinematicCharacterDeferredImpulse> deferredImpulsesBuffer,
                DynamicBuffer<KinematicVelocityProjectionHit>    velocityProjectionHits)
            {
                var characterProcessor = new ThirdPersonCharacterProcessor()
                                         {
                                             CharacterDataAccess = new KinematicCharacterDataAccess(

                                                  entity,
                                                  localTransform,
                                                  characterProperties,
                                                  characterBody,
                                                  physicsCollider,
                                                  characterHitsBuffer,
                                                  statefulHitsBuffer,
                                                  deferredImpulsesBuffer,
                                                  velocityProjectionHits
                                                 ),
                                             CharacterComponent = characterComponent,
                                             CharacterControl   = characterControl
                                         };

                characterProcessor.PhysicsUpdate(ref Context, ref BaseContext);
            }

            public bool OnChunkBegin(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask,
                                     in v128           chunkEnabledMask)
            {
                BaseContext.EnsureCreationOfTmpCollections();
                return true;
            }

            public void OnChunkEnd(in ArchetypeChunk chunk,            int  unfilteredChunkIndex, bool useEnabledMask,
                                   in v128           chunkEnabledMask, bool chunkWasExecuted)
            {
            }
        }
    }
}