using System;
using Sandbox.Diagnostics;

namespace CardGame;

public sealed class SceneManager : Singleton<SceneManager>, ISceneLoadingEvents
{
	[Property] public SceneFile? MenuScene { get; set; }
	
	[Property] public SceneFile? MapScene { get; set; }
	
	[Property] public SceneFile? BattleScene { get; set; }

	public event System.Action<Scene>? OnSceneLoaded;
	
	public static SceneFile? CurrentScene => (SceneFile)Game.ActiveScene.Source;
	
	private static readonly Logger Log = new( "SceneManager" );

	public void AfterLoad( Scene scene )
	{
		OnSceneLoaded?.Invoke( scene );
	}

	public void LoadScene( Scenes scene )
	{
		switch ( scene )
		{
			case Scenes.Menu:
				Scene.Load( MenuScene );
				break;
			case Scenes.Map:
				Scene.Load( MapScene );
				break;
			case Scenes.Battle:
				Scene.Load( BattleScene );
				break;
			default:
				throw new ArgumentOutOfRangeException( nameof( scene ), scene, null );
		}
	}

	public enum Scenes
	{
		Menu,
		Map,
		Battle
	}
}
