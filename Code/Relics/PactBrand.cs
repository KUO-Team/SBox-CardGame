using CardGame.Modifiers;
using CardGame.Units;

namespace CardGame.Relics;

public class PactBrand( Data.Relic data ) : Relic( data )
{
	private PowerModifier? _powerModifier;
	
	public override void BeforePlayCard( Card card, BattleUnit unit )
	{
		if ( unit != Owner )
		{
			return;
		}

		if ( card.Type != Card.CardType.Attack )
		{
			return;
		}

		Owner?.HealthComponent?.TakeFixedDamage( 1 );
		_powerModifier = new PowerModifier( 2, x => x.Type == Action.ActionType.Attack, 1 );
		card.Modifiers.AddModifier( _powerModifier );
		base.BeforePlayCard( card, unit );
	}

	public override void OnPlayCard( Card card, BattleUnit unit )
	{
		if ( _powerModifier is null )
		{
			return;
		}

		card.Modifiers.RemoveModifier( _powerModifier );
		base.OnPlayCard( card, unit );
	}
}
