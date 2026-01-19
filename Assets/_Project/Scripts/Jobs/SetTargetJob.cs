using AndrzejKebab.Components;
using AndrzejKebab.Components.Tags;
using Unity.Burst;
using Unity.Entities;

namespace AndrzejKebab.Jobs
{
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