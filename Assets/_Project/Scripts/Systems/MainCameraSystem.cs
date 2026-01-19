using AndrzejKebab.Components;
using Unity.Entities;
using Unity.Transforms;

namespace AndrzejKebab.Systems
{
    
	[UpdateInGroup(typeof(PresentationSystemGroup))]
	public partial class MainCameraSystem : SystemBase
	{
		protected override void OnUpdate()
		{
			if (MainCameraGameObject.Instance == null || !SystemAPI.HasSingleton<MainCameraEntityTag>()) return;
			Entity       mainEntityCameraEntity = SystemAPI.GetSingletonEntity<MainCameraEntityTag>();
			var targetLocalToWorld     = SystemAPI.GetComponent<LocalToWorld>(mainEntityCameraEntity);
			MainCameraGameObject.Instance.transform.SetPositionAndRotation(targetLocalToWorld.Position, targetLocalToWorld.Rotation);
		}
	}
}