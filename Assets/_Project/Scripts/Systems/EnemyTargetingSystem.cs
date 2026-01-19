using AndrzejKebab.Components;
using AndrzejKebab.Components.Tags;
using Unity.Burst;
using Unity.Entities;

namespace AndrzejKebab.Systems
{
	[BurstCompile]
	public partial struct EnemyTargetingSystem : ISystem
	{
		public void OnCreate(ref SystemState state)
		{
			state.RequireForUpdate<PlayerTag>();
		}

		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			if (!SystemAPI.HasSingleton<PlayerTag>()) return;

			Entity playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();

			var job = new SetTargetJob
			          {
				          TargetEntity = playerEntity
			          };

			state.Dependency = job.ScheduleParallelByRef(state.Dependency);
		}
		
		[BurstCompile]
		public partial struct SetTargetJob : IJobEntity
		{
			public Entity TargetEntity;

			private void Execute(ref TargetComponent target, in EnemyTag tag)
			{
				target.TargetEntity = TargetEntity;
			}
		}
	}
}