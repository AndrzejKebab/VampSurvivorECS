using Unity.Burst;
using Unity.Entities;

namespace AndrzejKebab.Systems
{
    
	[UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderLast = true)]
	[BurstCompile]
	public partial struct FixedTickSystem : ISystem
	{
		[BurstCompile]
		public struct Singleton : IComponentData
		{
			public uint Tick;
		}

		[BurstCompile]
		public void OnCreate(ref SystemState state)
		{
			if (SystemAPI.HasSingleton<Singleton>()) return;
			Entity singletonEntity = state.EntityManager.CreateEntity();
			state.EntityManager.AddComponentData(singletonEntity, new Singleton());
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			ref Singleton singleton = ref SystemAPI.GetSingletonRW<Singleton>().ValueRW;
			singleton.Tick++;
		}
	}
}