using AndrzejKebab.Components;
using Unity.Burst;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Transforms;

namespace AndrzejKebab.Systems
{
	[UpdateInGroup(typeof(SimulationSystemGroup))]
	[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
	[UpdateAfter(typeof(PlayerVariableStepControlSystem))]
	[UpdateAfter(typeof(CharacterVariableUpdateSystem))]
	[UpdateBefore(typeof(TransformSystemGroup))]
	[BurstCompile]
	public partial struct CameraSimulationSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<CameraComponent, CameraControlComponent>()
			                                .Build());
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			var job = new Jobs.CameraSimulationJob
			          {
				          DeltaTime                    = SystemAPI.Time.DeltaTime,
				          LocalTransformLookup         = SystemAPI.GetComponentLookup<LocalTransform>(),
				          ParentLookup                 = SystemAPI.GetComponentLookup<Parent>(true),
				          PostTransformMatrixLookup    = SystemAPI.GetComponentLookup<PostTransformMatrix>(true),
				          CameraTargetLookup           = SystemAPI.GetComponentLookup<CameraTargetComponent>(true),
				          KinematicCharacterBodyLookup = SystemAPI.GetComponentLookup<KinematicCharacterBody>(true)
			          };
			job.Schedule();
		}
	}
}