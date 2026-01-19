using AndrzejKebab.Components;
using Unity.Burst;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace AndrzejKebab.Systems
{
	[UpdateInGroup(typeof(SimulationSystemGroup))]
	[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
	[UpdateAfter(typeof(PlayerVariableStepControlSystem))]
	[UpdateBefore(typeof(TransformSystemGroup))]
	[BurstCompile]
	public partial struct CharacterVariableUpdateSystem : ISystem
	{
		private EntityQuery                       characterQuery;
		private CharacterUpdateContext context;
		private KinematicCharacterUpdateContext   baseContext;

		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<PhysicsWorldSingleton>();
			characterQuery = KinematicCharacterUtilities.GetBaseCharacterQueryBuilder()
			                                            .WithAll<
				                                            CharacterComponent,
				                                            CharacterControlComponent>()
			                                            .Build(ref state);

			context = new CharacterUpdateContext();
			context.OnSystemCreate(ref state);
			baseContext = new KinematicCharacterUpdateContext();
			baseContext.OnSystemCreate(ref state);

			state.RequireForUpdate(characterQuery);
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			context.OnSystemUpdate(ref state);
			baseContext.OnSystemUpdate(ref state, SystemAPI.Time, SystemAPI.GetSingleton<PhysicsWorldSingleton>());

			var job = new Jobs.CharacterVariableUpdateJob
			          {
				          Context     = context,
				          BaseContext = baseContext
			          };
			job.ScheduleParallel();
		}
	}
}