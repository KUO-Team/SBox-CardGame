using Sandbox.UI;
using Sandbox.Diagnostics;
using CardGame.Data;

namespace CardGame.UI;

public partial class MainMenu
{
	private RelicGainPanel? _relicGainPanel;
	private RelicSelectionPanel? _relics;
	private ClassSelectionPanel? _classes;
	private SettingsPanel? _settings;
	private LoadRunPanel? _runs;
	private StatisticsPanel? _statistics;
	private CreditsPanel? _credits;

	private Panel? _webContainer;
	private WebPanel? _webPanel;

	private static GameManager? GameManager => GameManager.Instance;
	private static MapManager? MapManager => MapManager.Instance;
	private static SaveManager? SaveManager => SaveManager.Instance;
	private static SceneManager? SceneManager => SceneManager.Instance;
	private static RelicManager? RelicManager => RelicManager.Instance;

	private static readonly Logger Log = new( "MainMenu" );
	
	private void NewRun()
	{
		if ( !MapManager.IsValid() )
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
		Classes();
	}

	public void Classes()
	{
		if ( !_classes.IsValid() )
		{
			return;
		}
		
		_classes.Show();
	}
	
	public void Relics()
	{
		if ( !_relics.IsValid() )
		{
			return;
		}

		_relics.Show();
	}

	public void StartRun( List<Relic>? relics = null )
	{
		if ( !RelicManager.IsValid() )
		{
			return;
		}
		
		RelicManager.ClearRelics();

		if ( relics is null )
		{
			StartRun();
		}
		else
		{
			foreach ( var relic in relics )
			{
				RelicManager.AddRelic( relic );
				if ( _relicGainPanel.IsValid() )
				{
					_relicGainPanel.Show( relic, StartRun );
				}
			}
		}
	}

	private void StartRun()
	{
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
	
	public static void Tutorial()
	{
		if ( RelicManager.IsValid() )
		{
			RelicManager.ClearRelics();
		}
		
		var tutorialBattle = BattleDataList.GetById( 1 );
		if ( tutorialBattle is not null )
		{
			PlayerData.Data.FirstTime = false;
			PlayerData.Save();
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
