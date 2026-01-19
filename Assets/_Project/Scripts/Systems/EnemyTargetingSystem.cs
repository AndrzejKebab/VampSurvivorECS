using AndrzejKebab.Components.Tags;
using AndrzejKebab.Jobs;
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
			var   isPlayerDead = SystemAPI.HasComponent<IsDeadTag>(playerEntity);
			
			var job = new SetTargetJob
			          {
				          TargetEntity = isPlayerDead ? Entity.Null : playerEntity
			          };

			state.Dependency = job.ScheduleParallelByRef(state.Dependency);
		}
	}
}