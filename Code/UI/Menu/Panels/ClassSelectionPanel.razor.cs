using System;
using Sandbox.UI;
using CardGame.Data;

namespace CardGame.UI;

public partial class ClassSelectionPanel
{
	private static PlayerClass? SelectedClass => Player.Local?.Class;
	
	private static GameManager? GameManager => GameManager.Instance;
	private static MapManager? MapManager => MapManager.Instance;
	private static SaveManager? SaveManager => SaveManager.Instance;
	private static RelicManager? RelicManager => RelicManager.Instance;
	private static SceneManager? SceneManager => SceneManager.Instance;
	
	private static void SelectClass( PlayerClass playerClass )
	{
		Player.Local?.SetClass( playerClass );
	}
	
	public static void StartRun()
	{
		if ( RelicManager.IsValid() )
		{
			RelicManager.ClearRelics();
		}
		
		if ( GameManager.IsValid() )
		{
			GameManager.Floor = GameManager.StartingFloor;
		}
		
		if ( SaveManager.IsValid() )
		{
			SaveManager.ClearActiveRun();
			SaveManager.ActiveRunData = new RunData();
		}
		
		if ( MapManager.IsValid() )
		{
			MapManager.Index = 0;
			MapManager.Seed = Game.Random.Next();
		}
		
		Log.Info( $"Starting new run" );
		Sandbox.Services.Stats.Increment( "runs", 1 );
		SceneManager?.LoadScene( SceneManager.Scenes.Map );
	}
	
	public void Close()
	{
		this.Hide();
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Player.Local?.Class );
	}
}
