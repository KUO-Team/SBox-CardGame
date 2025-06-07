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
	private static List<Id> SeenCards => Data.SeenCards;
	private static List<Id> SeenRelics => Data.SeenRelics;

	private Stats.GlobalStats? _globalStats;
	private Stats.PlayerStats? _playerStats;

	private List<StatEntry> _globalStatEntries = [];
	private List<StatEntry> _localStatEntries = [];

	private Dictionary<string, Leaderboards.Board2> _leaderboards = new()
	{
		{
			"Runs Won", Leaderboards.GetFromStat( Game.Ident, "game-over-win" )
		},
		{
			"Runs", Leaderboards.GetFromStat( Game.Ident, "runs" )
		}
	};

	private Panel? _tabContainer;
	private Panel? _leaderboardContainer;
	private Panel? _openLeaderboard;

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
		_globalStats.Refresh();
		_playerStats.Refresh();
		StateHasChanged();

		foreach ( var value in _leaderboards.Values )
		{
			value.Refresh();
		}

		if ( !_tabContainer.IsValid() )
		{
			return;
		}

		if ( !_leaderboardContainer.IsValid() )
		{
			return;
		}

		HideAllTabs();
		HideAllLeaderboards();

		var firstTab = _tabContainer.Children.FirstOrDefault();
		if ( firstTab.IsValid() )
		{
			ChangeTabs( firstTab.Id );
		}

		var firstLeaderboard = _leaderboardContainer.Children.FirstOrDefault();
		if ( firstLeaderboard.IsValid() )
		{
			ChangeLeaderboard( firstLeaderboard.Id );
		}

		base.OnAfterTreeRender( firstTime );
	}

	public void ChangeTabs( string id )
	{
		if ( !_tabContainer.IsValid() )
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
		if ( !_leaderboardContainer.IsValid() )
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
		return _tabContainer?.Children.FirstOrDefault( tab => tab.Id == id );
	}

	public Panel? GetLeaderboardById( string id )
	{
		return _leaderboardContainer?.Children.FirstOrDefault( tab => tab.Id == id );
	}

	private static void RefreshLeaderboard( Leaderboards.Board2 board )
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

	private static List<StatEntry> CreateStatEntries( Stats.GlobalStats? globalStats )
	{
		if ( globalStats is null )
		{
			return [];
		}

		return
		[
			new StatEntry( "Runs", globalStats.Get( "runs" ).Value.ToString( CultureInfo.CurrentCulture ) ),
			new StatEntry( "Runs Lost", globalStats.Get( "game-over-loss" ).Value.ToString( CultureInfo.CurrentCulture ) ),
			new StatEntry( "Runs Won", globalStats.Get( "game-over-win" ).Value.ToString( CultureInfo.CurrentCulture ) )
		];
	}

	private static List<StatEntry> CreateStatEntries( Stats.PlayerStats? playerStats )
	{
		if ( playerStats is null )
		{
			return [];
		}

		return
		[
			new StatEntry( "Runs", playerStats.Get( "runs" ).Value.ToString( CultureInfo.CurrentCulture ) ),
			new StatEntry( "Runs Lost", playerStats.Get( "game-over-loss" ).Value.ToString( CultureInfo.CurrentCulture ) ),
			new StatEntry( "Runs Won", playerStats.Get( "game-over-win" ).Value.ToString( CultureInfo.CurrentCulture ) )
		];
	}

	[ConCmd]
	private static void ShowAll()
	{
		foreach ( var card in CardDataList.All )
		{
			SeenCards.Add( card.Id );
		}

		foreach ( var relic in RelicDataList.All )
		{
			SeenRelics.Add( relic.Id );
		}
	}

	private void HideAllTabs()
	{
		if ( !_tabContainer.IsValid() )
		{
			return;
		}

		foreach ( var tab in _tabContainer.Children )
		{
			tab?.Hide();
		}
	}

	private void HideAllLeaderboards()
	{
		if ( !_leaderboardContainer.IsValid() )
		{
			return;
		}

		foreach ( var tab in _leaderboardContainer.Children )
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
		return HashCode.Combine( Data, Data.Runs.Count, Data.SeenCards.Count, _globalStatEntries.Count, _localStatEntries.Count );
	}

	public class StatEntry
	{
		public string Name { get; }
		public string Value { get; }

		// ReSharper disable once ConvertToPrimaryConstructor
		public StatEntry( string name, string value )
		{
			Name = name;
			Value = value;
		}
	}
}
