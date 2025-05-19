using CardGame.Modifiers;
using CardGame.Units;

namespace CardGame.Relics;

public class PactBrand( Data.Relic data ) : Relic( data )
{
	public override void OnPlayCard( Card card, BattleUnit unit )
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
		card.Modifiers.AddModifier( new PowerModifier( 2, x => x.Type == Action.ActionType.Attack, 1 ) );
	}
}
