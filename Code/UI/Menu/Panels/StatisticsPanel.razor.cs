using System;
using System.Globalization;
using Sandbox.UI;
using Sandbox.Services;
using CardGame.Data;

namespace CardGame.UI;

public partial class StatisticsPanel
{
	public Panel? SelectedTab { get; private set; }

	private static PlayerData Data => PlayerData.Data;
	private Panel? _tabs;

	private Stats.GlobalStats? _globalStats;
	private Stats.PlayerStats? _playerStats;

	private List<StatEntry> _globalStatEntries = [];
	private List<StatEntry> _localStatEntries = [];

	private const string GameIdent = "spoonstuff.card_game";

	private Dictionary<string, Leaderboards.Board2> _leaderboards = new()
	{
		{
			"Runs Won", Leaderboards.GetFromStat( GameIdent, "game-over-win" )
		},
		{
			"Runs", Leaderboards.GetFromStat( GameIdent, "runs" )
		}
	};

	private Panel? _openLeaderboard;
	private Panel? _l;

	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime )
		{
			return;
		}

		_globalStats = Stats.Global;
		_playerStats = Stats.LocalPlayer;

		_globalStatEntries = CreateStatEntries( _globalStats );
		_localStatEntries = CreateStatEntries( _playerStats );

		foreach ( var value in _leaderboards.Values )
		{
			value.Refresh();
		}

		if ( !_tabs.IsValid() )
		{
			return;
		}

		if ( !_l.IsValid() )
		{
			return;
		}

		HideAllTabs();
		HideAllLeaderboards();

		var firstTab = _tabs.Children.FirstOrDefault();
		if ( firstTab.IsValid() )
		{
			ChangeTabs( firstTab.Id );
		}
		
		var firstLeaderboard = _l.Children.FirstOrDefault();
		if ( firstLeaderboard.IsValid() )
		{
			ChangeLeaderboard( firstLeaderboard.Id );
		}

		base.OnAfterTreeRender( firstTime );
	}

	public void ChangeTabs( string id )
	{
		if ( !_tabs.IsValid() )
		{
			return;
		}

		var selectedTab = GetTabById( id );
		HideAllTabs();

		if ( selectedTab.IsValid() )
		{
			selectedTab.Show();
			SelectedTab = selectedTab;
		}
	}

	public void ChangeLeaderboard( string id )
	{
		if ( !_l.IsValid() )
		{
			return;
		}
		
		var selectedLeaderboard = GetLeaderboardById( id );
		HideAllLeaderboards();

		if ( selectedLeaderboard.IsValid() )
		{
			selectedLeaderboard.Show();
			_openLeaderboard = selectedLeaderboard;
		}
	}

	public Panel? GetTabById( string id )
	{
		return _tabs?.Children.FirstOrDefault( tab => tab.Id == id );
	}

	public Panel? GetLeaderboardById( string id )
	{
		return _l?.Children.FirstOrDefault( tab => tab.Id == id );
	}

	public void RefreshLeaderboard( Leaderboards.Board2 board )
	{
		board.Refresh();
		board.CenterOnMe();
	}
	
	private static string GetRank( int position )
	{
		return position switch
		{
			1 => "gold",
			2 => "silver",
			3 => "bronze",
			_ => string.Empty
		};
	}

	private static bool IsMe( SteamId id )
	{
		return id == Connection.Local.SteamId;
	}

	private List<StatEntry> CreateStatEntries( Stats.GlobalStats? globalStats )
	{
		if ( globalStats is null )
		{
			return [];
		}

		return [
			new StatEntry( "Runs", globalStats.Get( "runs" ).Value.ToString( CultureInfo.CurrentCulture ) ),
			new StatEntry( "Runs Lost", globalStats.Get( "game-over-loss" ).Value.ToString( CultureInfo.CurrentCulture ) ),
			new StatEntry( "Runs Won", globalStats.Get( "game-over-win" ).Value.ToString( CultureInfo.CurrentCulture ) )
		];
	}

	private List<StatEntry> CreateStatEntries( Stats.PlayerStats? playerStats )
	{
		if ( playerStats is null )
		{
			return [];
		}

		return [
			new StatEntry( "Runs", playerStats.Get( "runs" ).Value.ToString( CultureInfo.CurrentCulture ) ),
			new StatEntry( "Runs Lost", playerStats.Get( "game-over-loss" ).Value.ToString( CultureInfo.CurrentCulture ) ),
			new StatEntry( "Runs Won", playerStats.Get( "game-over-win" ).Value.ToString( CultureInfo.CurrentCulture ) )
		];
	}

	private void HideAllTabs()
	{
		if ( !_tabs.IsValid() )
		{
			return;
		}

		foreach ( var tab in _tabs.Children )
		{
			tab?.Hide();
		}
	}

	private void HideAllLeaderboards()
	{
		if ( !_l.IsValid() )
		{
			return;
		}

		foreach ( var tab in _l.Children )
		{
			tab?.Hide();
		}
	}

	public string GetAvailabilityText( Card.CardAvailabilities availability )
	{
		return availability switch
		{
			Card.CardAvailabilities.None => "Not available.",
			Card.CardAvailabilities.Shop => "Found in the shop.",
			Card.CardAvailabilities.Starter => "Found in the starting deck.",
			Card.CardAvailabilities.Reward => "Found as a reward.",
			Card.CardAvailabilities.Chest => "Found in a chest.",
			Card.CardAvailabilities.Event => "Found in an event.",
			Card.CardAvailabilities.DevOnly => "Developer only.",
			_ => throw new ArgumentOutOfRangeException( nameof( availability ), availability, null )
		};
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Data, Data.Runs.Count, Data.SeenCards.Count );
	}

	public class StatEntry
	{
		public string Name { get; }
		public string Value { get; }

		public StatEntry( string name, string value )
		{
			Name = name;
			Value = value;
		}
	}
}
