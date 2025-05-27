using System;
using Sandbox.Diagnostics;
using CardGame.Data;

namespace CardGame;

public sealed class SaveManager : Singleton<SaveManager>
{
	[Property, InlineEditor]
	public RunData? ActiveRunData { get; set; }

	private static PlayerData Data => PlayerData.Data;

	private static GameManager? GameManager => GameManager.Instance;
	private static MapManager? MapManager => MapManager.Instance;
	private static SceneManager? SceneManager => SceneManager.Instance;

	private static readonly Logger Log = new( "SaveManager" );

	public void Save( int? index = null )
	{
		if ( !GameManager.IsValid() )
		{
			return;
		}

		if ( !MapManager.IsValid() )
		{
			return;
		}

		RunData run;
		var actualIndex = 0;
		if ( index.HasValue )
		{
			run = Data.Runs.FirstOrDefault( x => x.Index == index )
				?? throw new InvalidOperationException( "Run with specified index not found." );

			actualIndex = run.Index;
		}
		else
		{
			actualIndex = Data.Runs.Count != 0 ? Data.Runs.Max( x => x.Index ) + 1 : 0;
			run = new RunData();
			Data.Runs.Add( run );
		}

		run.Version = GameInfo.Version;
		run.Index = actualIndex;
		run.Floor = GameManager.Floor;
		run.Seed = MapManager.Seed;
		run.MapNodeIndex = MapManager.Index;

		var player = Player.Local;
		if ( player.IsValid() )
		{
			run.Money = player.Money;
			if ( player.Class is not null )
			{
				run.Class = player.Class.Id;
			}

			var playerUnit = player.Unit;
			if ( playerUnit is not null )
			{
				run.UnitData = playerUnit;
			}

			foreach ( var card in player.Cards )
			{
				run.Cards.Add( card.Id );
			}

			foreach ( var cardPack in player.CardPacks )
			{
				run.CardPacks.Add( cardPack.Id );
			}
		}

		if ( RelicManager.Instance.IsValid() )
		{
			foreach ( var relic in RelicManager.Instance.Relics )
			{
				run.Relics.Add( relic.Data.Id );
			}
		}

		PlayerData.Save();
	}

	public void Load( RunData data )
	{
		if ( !GameManager.IsValid() )
		{
			return;
		}

		if ( !MapManager.IsValid() )
		{
			return;
		}

		if ( !SceneManager.IsValid() )
		{
			return;
		}

		ActiveRunData = data;
		MapManager.Seed = data.Seed;
		MapManager.Index = data.MapNodeIndex;
		GameManager.Floor = data.Floor;

		var player = Player.Local;
		if ( player.IsValid() )
		{
			player.Money = data.Money;
			player.SetClassById( data.Class );

			var playerUnit = player.Unit;
			if ( playerUnit is not null )
			{
				playerUnit.Hp = data.UnitData.Hp;
				playerUnit.Deck.Clear();

				foreach ( var cardId in data.UnitData.Deck )
				{
					playerUnit.Deck.Add( cardId );
				}
			}

			foreach ( var cardId in data.Cards )
			{
				var card = CardDataList.GetById( cardId );
				if ( card is not null )
				{
					player.Cards.Add( card );
				}
			}

			foreach ( var packId in data.CardPacks )
			{
				var pack = CardPackDataList.GetById( packId );
				if ( pack is not null )
				{
					player.CardPacks.Add( pack );
				}
			}
		}
		
		RelicManager.Instance?.ClearRelics();
		foreach ( var relicId in data.Relics )
		{
			var relic = RelicDataList.GetById( relicId );
			if ( relic is not null )
			{
				RelicManager.Instance?.AddRelic( relic );
			}
		}

		Scene.Load( SceneManager.MapScene );
	}

	public void Delete( int index )
	{
		var run = Data.Runs.FirstOrDefault( x => x.Index == index );
		if ( run is null )
		{
			Log.Error( "Run not found!" );
			return;
		}

		Data.Runs.Remove( run );
		PlayerData.Save();
	}

	public void ClearActiveRun()
	{
		if ( ActiveRunData is null )
		{
			Log.Warning( $"Unable to clear active run data; no data set!" );
			return;
		}

		ActiveRunData.MapNodeIndex = 0;
		ActiveRunData.Floor = GameManager.StartingFloor;
		if ( MapManager.Instance.IsValid() )
		{
			MapManager.Instance.Index = 0;
		}

		Player.Local?.CardPacks.Clear();
		Player.Local?.Cards.Clear();
		Player.Local?.ResetUnitData();

		ActiveRunData = null;
		Log.Info( $"Cleared active run data." );
	}
}
