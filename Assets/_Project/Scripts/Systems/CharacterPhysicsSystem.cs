using Unity.Burst;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Physics;

namespace AndrzejKebab.Systems
{

	[UpdateInGroup(typeof(KinematicCharacterPhysicsUpdateGroup))]
	public partial struct CharacterPhysicsSystem : ISystem
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

			var job = new Jobs.CharacterPhysicsUpdateJob
			          {
				          Context     = context,
				          BaseContext = baseContext
			          };
			job.ScheduleParallel();
		}
	}
}