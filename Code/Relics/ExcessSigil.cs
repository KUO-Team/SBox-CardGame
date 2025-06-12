using System;
using CardGame.Units;
using CardGame.Modifiers;

namespace CardGame.Relics;

public class ExcessSigil( Data.Relic data ) : Relic( data )
{
	private ICardModifier? _modifier;

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
					_modifier = new EffectPowerModifier( 2,1 );
					card.Modifiers.AddModifier( _modifier );
					break;
				}
			case Card.CardType.Attack:
				{
					_modifier = new ActionPowerModifier( -2, action => action.Type == Action.ActionType.Attack, 1 );
					card.Modifiers.AddModifier( _modifier );
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
		if ( _modifier is null )
		{
			return;
		}

		card.Modifiers.RemoveModifier( _modifier );
		base.OnPlayCard( card, unitComponent );
	}
}
