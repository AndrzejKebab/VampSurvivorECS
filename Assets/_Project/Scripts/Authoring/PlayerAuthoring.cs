using AndrzejKebab.Components;
using AndrzejKebab.Components.Tags;
using Unity.Entities;
using UnityEngine;

namespace AndrzejKebab.Authoring;

public class PlayerAuthoring : MonoBehaviour
{
	[Header("Links")] 
	public GameObject ControlledCharacter;
	public GameObject ControlledCamera;

	public class Baker : Baker<PlayerAuthoring>
	{
		public override void Bake(PlayerAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.None);
			
			AddComponent(entity, new PlayerReferencesComponent
			                     {
				                     ControlledCharacter =
					                     GetEntity(authoring.ControlledCharacter, TransformUsageFlags.Dynamic),
				                     ControlledCamera =
					                     GetEntity(authoring.ControlledCamera, TransformUsageFlags.Dynamic)
			                     });

			AddComponent(entity, new PlayerInputsComponent());
		}
	}
}