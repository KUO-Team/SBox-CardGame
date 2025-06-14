﻿using CardGame.Data;

namespace CardGame;

public static class Commands
{
	[ConCmd]
	public static void SetMoney( int amount )
	{
		if ( !Game.IsEditor && !Game.CheatsEnabled )
		{
			return;
		}

		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}

		player.Money = amount;
		Platform.Platform.CheatedRun = true;
	}

	[ConCmd]
	public static void AddMoney( int amount )
	{
		if ( !Game.IsEditor && !Game.CheatsEnabled )
		{
			return;
		}

		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}

		player.Money += amount;
		Platform.Platform.CheatedRun = true;
	}
	
	[ConCmd]
	public static void DrawCard( int id )
	{
		if ( !Game.IsEditor && !Game.CheatsEnabled )
		{
			return;
		}

		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}

		var unit = player.Units.FirstOrDefault();
		if ( !unit.IsValid() )
		{
			return;
		}

		var card = CardDataList.GetById( id );
		if ( card is null )
		{
			return;
		}
		
		var copy = card.DeepCopy();
		unit.HandComponent?.Draw( copy );
		
		Platform.Platform.CheatedRun = true;
	}
	
	[ConCmd]
	public static void AddCard( int id )
	{
		if ( !Game.IsEditor && !Game.CheatsEnabled )
		{
			return;
		}

		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}

		var card = CardDataList.GetById( id );
		if ( card is null )
		{
			return;
		}

		player.Cards.Add( card );
		Platform.Platform.CheatedRun = true;
	}
	
	[ConCmd]
	public static void AddRelic( int id )
	{
		if ( !Game.IsEditor && !Game.CheatsEnabled )
		{
			return;
		}

		if ( !RelicManager.Instance.IsValid() )
		{
			return;
		}

		var relic = RelicDataList.GetById( id );
		if ( relic is not null )
		{
			RelicManager.Instance.AddRelic( relic );
		}
		Platform.Platform.CheatedRun = true;
	}

	[ConCmd]
	public static async void StartBattle( int id )
	{
		if ( !Game.IsEditor && !Game.CheatsEnabled )
		{
			return;
		}

		if ( !BattleManager.Instance.IsValid() )
		{
			return;
		}
		
		var battle = BattleDataList.GetById( id );
		if ( battle is null )
		{
			return;
		}

		await BattleManager.Instance.StartBattle( battle );
	}
}
