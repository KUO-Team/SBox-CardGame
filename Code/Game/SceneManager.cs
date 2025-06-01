using System;
using Sandbox.Diagnostics;

namespace CardGame;

public sealed class SceneManager : Singleton<SceneManager>, ISceneLoadingEvents
{
	[Property] 
	public SceneFile? MenuScene { get; set; }
	
	[Property] 
	public SceneFile? MapScene { get; set; }
	
	[Property] 
	public SceneFile? BattleScene { get; set; }

	public event Action<Scene>? OnSceneLoaded;
	
	public static SceneFile? CurrentScene => (SceneFile)Game.ActiveScene.Source;
	
	private static readonly Logger Log = new( "SceneManager" );

	public void AfterLoad( Scene scene )
	{
		OnSceneLoaded?.Invoke( scene );
	}

	public bool LoadScene( Scenes scene, bool destroyPersistentObjects = false )
	{
		if ( scene != Scenes.Battle && BattleManager.Instance.IsValid() )
		{
			BattleManager.Instance.ForceEndBattle();
		}

		if ( destroyPersistentObjects )
		{
			Scene.DestroyPersistentObjects();
		}
		
		return scene switch
		{
			Scenes.Menu => Scene.Load( MenuScene ),
			Scenes.Map => Scene.Load( MapScene ),
			Scenes.Battle => Scene.Load( BattleScene ),
			_ => throw new ArgumentOutOfRangeException( nameof( scene ), scene, null )
		};
	}

	public enum Scenes
	{
		Menu,
		Map,
		Battle
	}
}
