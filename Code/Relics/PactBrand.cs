﻿using CardGame.Modifiers;
using CardGame.Units;

namespace CardGame.Relics;

public class PactBrand( Data.Relic data ) : Relic( data )
{
	private ActionPowerModifier? _powerModifier;
	
	public override void BeforePlayCard( Card card, BattleUnitComponent unitComponent )
	{
		if ( unitComponent != Owner )
		{
			return;
		}

		if ( card.Type != Card.CardType.Attack )
		{
			return;
		}

		Owner?.HealthComponent?.TakeFixedDamage( 2 );
		_powerModifier = new ActionPowerModifier( 2, x => x.Type == Action.ActionType.Attack, 1 );
		card.Modifiers.AddModifier( _powerModifier );
		base.BeforePlayCard( card, unitComponent );
	}

	public override void OnPlayCard( Card card, BattleUnitComponent unitComponent )
	{
		if ( _powerModifier is null )
		{
			return;
		}

		card.Modifiers.RemoveModifier( _powerModifier );
		base.OnPlayCard( card, unitComponent );
	}
}
