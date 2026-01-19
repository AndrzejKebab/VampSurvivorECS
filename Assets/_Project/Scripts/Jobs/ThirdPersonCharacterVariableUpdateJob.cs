using AndrzejKebab.Components;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace AndrzejKebab.Jobs
{
	[BurstCompile]
	[WithAll(typeof(Simulate))]
	public partial struct ThirdPersonCharacterVariableUpdateJob : IJobEntity, IJobEntityChunkBeginEnd
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
			var characterProcessor = new ThirdPersonCharacterProcessor
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

			characterProcessor.VariableUpdate(ref Context, ref BaseContext);
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