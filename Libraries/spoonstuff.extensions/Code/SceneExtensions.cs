using System;
using System.Linq;

namespace Sandbox;

public static class SceneExtensions
{
	/// <summary>
	/// Searches the scene for any gameobjects with the DontDestroyOnLoad flag and destroys them.
	/// </summary>
	/// <param name="scene">The scene to search in.</param>
	/// <param name="ignored">Component types to ignore deletion.</param>
	public static void DestroyPersistentObjects( this Scene scene, Type[] ignored = null )
	{
		var objects = scene.GetAllObjects( true ).Where( x => x.Flags == GameObjectFlags.DontDestroyOnLoad );

		foreach ( var go in objects )
		{
			var hasIgnoredType = false;
			if ( ignored != null )
			{
				if ( ignored.Any( type => go.Components.Get( type ) != null ) )
				{
					hasIgnoredType = true;
				}
			}

			if ( !hasIgnoredType )
			{
				go.Destroy();
			}
		}
	}

	/// <summary>
	/// Search the scene for a specific component instance.
	/// </summary>
	/// <typeparam name="T">Component to search for.</typeparam>
	/// <param name="scene">The scene to search in.</param>
	/// <returns>Found component instance.</returns>
	/// <exception cref="ArgumentException">Thrown if no instance could be found.</exception>
	/// <exception cref="InvalidOperationException">Thrown if multiple instances are found.</exception>
	public static T GetInstance<T>( this Scene scene ) where T : Component
	{
		try
		{
			return scene.GetAllComponents<T>().SingleOrDefault();
		}
		catch ( ArgumentNullException )
		{
			throw new ArgumentException( $"Could not get an instance of type {typeof(T)}. None found!" );
		}
		catch ( InvalidOperationException )
		{
			throw new InvalidOperationException( $"Multiple instances of type {typeof(T)} found in the scene." );
		}
	}
}
