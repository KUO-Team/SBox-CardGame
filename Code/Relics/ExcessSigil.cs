using System;
using CardGame.Modifiers;
using CardGame.Units;

namespace CardGame.Relics;

public class ExcessSigil( Data.Relic data ) : Relic( data )
{
	public override void OnTurnStart()
	{
		if ( !Owner.IsValid() || !Owner.HandComponent.IsValid() )
		{
			return;
		}

		foreach ( var card in Owner.HandComponent.Hand )
		{
			switch ( card.Type )
			{
				case Card.CardType.Spell:
					{
						card.Modifiers.AddModifier( new PowerModifier( 2, action => action.Type == Action.ActionType.Effect, 1 ) );
						break;
					}
				case Card.CardType.Attack:
					{
						card.Modifiers.AddModifier( new PowerModifier( -2, action => action.Type == Action.ActionType.Attack, 1 ) );
						break;
					}
				case Card.CardType.Defense:
				case Card.CardType.Item:
					break;
				default:
					throw new ArgumentOutOfRangeException( card.Type.ToString() );
			}
		}

		base.OnTurnStart();
	}
}
