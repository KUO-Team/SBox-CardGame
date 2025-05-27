using System;
using Sandbox.Audio;
using Sandbox.Diagnostics;
using CardGame.Data;
using CardGame.Platform;
using CardGame.UI;

namespace CardGame;

public sealed class GameManager : Singleton<GameManager>
{
	[Property]
	public int Floor { get; set; } = StartingFloor;

	[Property]
	public RunOverPanel? RunOverPanel { get; set; }

	private static BattleManager? BattleManager => BattleManager.Instance;
	private static SceneManager? SceneManager => SceneManager.Instance;
	private static SaveManager? SaveManager => SaveManager.Instance;

	private static readonly Logger Log = new( "GameManager" );

	public const int StartingFloor = 3;

	protected override void OnStart()
	{
		GameObject.Flags = GameObjectFlags.DontDestroyOnLoad;
		PlayerData.Load();

		var settings = Settings.Data;
		Mixer.FindMixerByName( "Master" ).Volume = settings.MasterVolume;
		Mixer.FindMixerByName( "Music" ).Volume = settings.MusicVolume;
		Mixer.FindMixerByName( "Game" ).Volume = settings.GameVolume;
		Mixer.FindMixerByName( "UI" ).Volume = settings.UIVolume;

		foreach ( var card in CardDataList.All )
		{
			card.Actions.ForEach( x =>
			{
				x.Card = card;
				x.InitEffect();
			} );
		}
		PlayerData.Data.ValidateSeenCards();
		PlayerData.Data.ValidateSeenRelics();

		base.OnStart();
	}

	public void OnBattleEnd( Faction winner, System.Action? callback = null )
	{
		if ( !BattleManager.IsValid() )
		{
			return;
		}

		if ( BattleManager.Battle is null )
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

		if ( winner == Faction.Player )
		{
			GiveRewards( BattleManager.Battle );

			if ( SaveManager.ActiveRunData is {} runData )
			{
				var wIndex = SaveManager.ActiveRunData.MapNodeIndex;
				if ( !runData.CompletedNodes.Contains( wIndex ) )
				{
					runData.CompletedNodes.Add( wIndex );
				}
				PlayerData.Save();
			}

			Scene.Load( SceneManager.MapScene );
		}

		callback?.Invoke();
	}

	public static void GiveRewards( Battle battle )
	{
		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}

		foreach ( var cardId in battle.Rewards.Cards )
		{
			var card = CardDataList.GetById( cardId );
			if ( card is null )
			{
				continue;
			}

			player.Cards.Add( card );
			Log.Info( $"Awarded card {card}" );
		}

		foreach ( var packId in battle.Rewards.CardPacks )
		{
			PlayerData.Data.CardPacks.Add( packId );
			var pack = CardPackDataList.GetById( packId );
			if ( pack is null )
			{
				continue;
			}

			player.CardPacks.Add( pack );
			Log.Info( $"Awarded cardpack {pack}" );
		}

		foreach ( var relicId in battle.Rewards.Relics )
		{
			var relic = RelicDataList.GetById( relicId );
			if ( relic is null )
			{
				continue;
			}

			var relicManager = RelicManager.Instance;
			if ( relicManager.IsValid() )
			{
				relicManager.AddRelic( relic );
			}
		}

		if ( Player.Local.IsValid() )
		{
			Player.Local.Money += battle.Rewards.Money;
		}
	}

	public void NextFloor()
	{
		if ( !MapManager.Instance.IsValid() )
		{
			return;
		}

		switch ( Floor )
		{
			case 3:
				Platform.Achievement.Floor3.Unlock();
				break;
			case 2:
				Platform.Achievement.Floor2.Unlock();
				break;
			case 1:
				Platform.Achievement.Floor1.Unlock();
				break;
			case 0:
				break;
		}

		Floor = Math.Max( Floor - 1, 0 );

		// Specifically + 1 to floor so we always have a valid new seed, even at floor 0.
		MapManager.Instance.Seed += Floor + 1;

		if ( Player.Local is {} player )
		{
			if ( player.Unit is null )
			{
				return;
			}

			player.Unit.HealToMax();
		}
	}

	public void EndRunInLoss()
	{
		if ( BattleManager.IsValid() )
		{
			BattleManager.ShowEndScreen = false;
		}

		if ( RunOverPanel.IsValid() )
		{
			RunOverPanel.EndRun( () =>
			{
				SaveManager?.ClearActiveRun();
			} );
			RunOverPanel.GameObject.Enabled = true;
		}

		Stats.Increment( "game-over-loss", 1 );
	}

	public void EndRunInWin()
	{
		Stats.Increment( "game-over-win", 1 );
		Platform.Achievement.Floor0.Unlock();
		SceneManager?.LoadScene( SceneManager.Scenes.Menu );
	}
}

public static class GameInfo
{
	public const string Version = "0.3";
	public const string Build = "Development Build";
}
