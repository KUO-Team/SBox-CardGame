using System;
using CardGame.Modifiers;
using CardGame.Units;

namespace CardGame.Relics;

public class ExcessSigil( Data.Relic data ) : Relic( data )
{
	private PowerModifier? _powerModifier;

	public override void BeforePlayCard( Card card, BattleUnitComponent unitComponent )
	{
		if ( unitComponent != Owner )
		{
			return;
		}
		
		switch ( card.Type )
		{
			case Card.CardType.Spell:
				{
					_powerModifier = new PowerModifier( 2, action => action.Type == Action.ActionType.Effect, 1 );
					card.Modifiers.AddModifier( _powerModifier );
					break;
				}
			case Card.CardType.Attack:
				{
					_powerModifier = new PowerModifier( -2, action => action.Type == Action.ActionType.Attack, 1 );
					card.Modifiers.AddModifier( _powerModifier );
					break;
				}
			case Card.CardType.Defense:
			case Card.CardType.Item:
				break;
			default:
				throw new ArgumentOutOfRangeException( card.Type.ToString() );
		}
		
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
