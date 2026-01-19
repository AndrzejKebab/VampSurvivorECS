using AndrzejKebab.Components;
using Unity.Burst;
using Unity.CharacterController;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace AndrzejKebab.Systems
{
	[UpdateInGroup(typeof(SimulationSystemGroup))]
	[UpdateAfter(typeof(TransformSystemGroup))]
	[BurstCompile]
	public partial struct OrbitCameraLateUpdateSystem : ISystem
	{
		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<PhysicsWorldSingleton>();
			state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<OrbitCameraComponent, OrbitCameraControl>().Build());
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			var job = new Jobs.OrbitCameraLateUpdateJob
			          {
				          DeltaTime                    = SystemAPI.Time.DeltaTime,
				          PhysicsWorld                 = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld,
				          LocalToWorldLookup           = SystemAPI.GetComponentLookup<LocalToWorld>(),
				          CameraTargetLookup           = SystemAPI.GetComponentLookup<CameraTarget>(true),
				          KinematicCharacterBodyLookup = SystemAPI.GetComponentLookup<KinematicCharacterBody>(true)
			          };
			job.Schedule();
		}
	}
}