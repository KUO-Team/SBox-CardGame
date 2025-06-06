﻿using System;
using CardGame.Data;
using CardGame.UI.Map;

namespace CardGame;

/// <summary>
/// Provides helper methods for events.
/// </summary>
public static class EventUtility
{
	public static void HealToMax()
	{
		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}

		player.Unit?.HealToMax();
	}

	public static void HealPlayer( int amount )
	{
		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}

		var unit = player.Unit;
		if ( unit is null )
		{
			return;
		}

		amount = Math.Max( 0, amount );
		unit.Damage( amount );
	}

	public static void TakePlayerDamage( int amount )
	{
		if ( amount <= 0 )
		{
			Log.Warning( $"Tried healing a negative amount! Did you mean {nameof( HealPlayer )}?" );
			return;
		}

		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}

		var unit = player.Unit;
		if ( unit is null )
		{
			return;
		}

		amount = Math.Max( 0, amount );
		unit.Damage( amount );
	}

	public static void AddMoney( int amount )
	{
		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}

		player.Money += amount;
	}

	public static void SubtractMoney( int amount, bool clamp = true )
	{
		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}

		if ( clamp )
		{
			player.Money = Math.Max( 0, player.Money - amount );
		}
		else
		{
			player.Money -= amount;
		}
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

	public static void AddCardPack( Id id )
	{
		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}

		var pack = CardPackDataList.GetById( id );
		if ( pack is not null )
		{
			player.CardPacks.Add( pack );
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

			var panel = Game.ActiveScene.GetInstance<MapPanel>();
			if ( !panel.IsValid() )
			{
				return;
			}

			if ( panel.RelicGainPanel.IsValid() )
			{
				panel.RelicGainPanel?.Show( relic );
			}
		}
		else
		{
			Log.Warning( $"Unable to add relic: no relic with ID {id} found!" );
		}
	}
}
