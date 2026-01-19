using Unity.Entities;

namespace AndrzejKebab;

public struct ThirdPersonCharacterUpdateContext
{
	// Here, you may add additional global data for your character updates, such as ComponentLookups, Singletons, NativeCollections, etc...
	// The data you add here will be accessible in your character updates and all of your character "callbacks".

	public void OnSystemCreate(ref SystemState state)
	{
		// Get lookups
	}

	public void OnSystemUpdate(ref SystemState state)
	{
		// Update lookups
	}
}