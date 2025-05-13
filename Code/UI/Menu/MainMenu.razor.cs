using Sandbox.UI;
using CardGame.Data;
using Sandbox.Diagnostics;

namespace CardGame.UI;

public partial class MainMenu
{
	private PackOpeningPanel? _packOpening;
	private SettingsPanel? _settings;
	private LoadRunPanel? _runs;
	private StatisticsPanel? _statistics;
	private CreditsPanel? _credits;

	private Panel? _webContainer;
	private WebPanel? _webPanel;
	
	private static GameManager? GameManager => GameManager.Instance;
	private static MapManager? MapManager => MapManager.Instance;
	private static SceneManager? SceneManager => SceneManager.Instance;
	private static SaveManager? SaveManager => SaveManager.Instance;

	private static readonly Logger Log = new( "MainMenu" );
	
	public void NewRun()
	{
		if ( !MapManager.IsValid() )
		{
			return;
		}

		if ( !SceneManager.IsValid() )
		{
			return;
		}

		if ( !SaveManager.IsValid() )
		{
			return;
		}
		
		Platform.Platform.CheatedRun = false;
		var firstTime = PlayerData.Data.FirstTime;
		if ( firstTime )
		{
			PlayerData.Data.FirstTime = false;
			PlayerData.Save();
			
			WarningPanel? warning = null;
			warning = WarningPanel.Create( "Tutorial", "It appears it is your first time playing the game! Would you like to play the tutorial?", [
				new Button( "Yes", "", () =>
				{
					warning?.Delete();
					Tutorial();
				} ),
				new Button( "No", "", () =>
				{
					warning?.Delete();
					StartNewRun();
				} )
			] );
		}
		else
		{
			StartNewRun();
		}
	}
	
	private void StartNewRun()
	{
		if ( !GameManager.IsValid() || !SaveManager.IsValid() || !MapManager.IsValid() || !SceneManager.IsValid() )
		{
			return;
		}
		
		RelicManager.Instance?.Relics.Clear();
		Log.Info( $"Starting new run" );

		GameManager.Floor = GameManager.StartingFloor;
		SaveManager.ClearActiveRun();
		SaveManager.ActiveRunData = new RunData();
		MapManager.Index = 0;
		MapManager.Seed = Game.Random.Next();
		Scene.Load( SceneManager.MapScene );
		Sandbox.Services.Stats.Increment( "runs", 1 );
	}

	private void Tutorial()
	{
		RelicManager.Instance?.Relics.Clear();
		var tutorialBattle = BattleDataList.GetById( 1 );
		if ( tutorialBattle is not null )
		{
			BattleManager.Instance?.StartBattle( tutorialBattle );
		}
		else
		{
			Log.Error( "Can't play tutorial battle; tutorial battle not found!" );
		}
	}
	
	public void LoadRun()
	{
		_runs?.Show();
	}
	
	public void Statistics()
	{
		_statistics?.Show();
	}
	
	public void OpenCredits()
	{
		_credits?.Show();
	}
	
	public void OpenSettings()
	{
		_settings?.Show();
	}

	public void ClearData()
	{
		WarningPanel? warning = null;
		warning = WarningPanel.Create( "Clear Data", "This action will clear all of your collection, player and run data. It will not clear your achievements, stats or leaderboard placements. This action is irreversible. Are you sure?", [
			new Button( "Yes", "", () =>
			{
				PlayerData.Clear();
				warning?.Delete();
			} ),
			new Button( "No", "", () =>
			{
				warning?.Delete();
			} )
		] );
	}

	public const string DiscordInvite = "https://discord.gg/kKU6a4AYNk";

	public void OpenWebPanel( string url )
	{
		if ( !_webContainer.IsValid() || !_webPanel.IsValid() )
		{
			return;
		}
		
		_webPanel.Url = url;
		_webContainer.Show();
	}

	public void CloseWebPanel()
	{
		if ( !_webContainer.IsValid() || !_webPanel.IsValid() )
		{
			return;
		}

		_webPanel.Url = string.Empty;
		_webContainer.Hide();
	}
}
