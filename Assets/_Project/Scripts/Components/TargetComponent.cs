using Unity.Entities;

namespace AndrzejKebab.Components
{
	public struct TargetComponent : IComponentData
	{
		public Entity TargetEntity;
	}
}