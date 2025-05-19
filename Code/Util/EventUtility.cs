using System;
using CardGame.Data;

namespace CardGame;

public static class EventUtility
{
	public static void AddMoney( int amount )
	{
		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}
		
		player.Money += amount;
	}
	
	public static void SubtractMoney( int amount )
	{
		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}
		
		player.Money = Math.Max( 0, player.Money - amount );
	}

	public static void AddCard( Id id )
	{
		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}
		
		var card = CardDataList.GetById( id );
		if ( card is not null )
		{
			PlayerData.Data.SeeCard( id );
			player.Cards.Add( card );
		}
		else
		{
			Log.Warning( $"Unable to add card: no card with ID {id} found!" );
		}
	}

	public static void AddRelic( Id id )
	{
		var relic = RelicDataList.GetById( id );
		if ( relic is not null )
		{
			PlayerData.Data.SeeRelic( id );
			RelicManager.Instance?.AddRelic( relic );
		}
		else
		{
			Log.Warning( $"Unable to add relic: no relic with ID {id} found!" );
		}
	}
}
