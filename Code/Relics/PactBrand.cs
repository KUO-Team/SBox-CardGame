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
		
		Owner?.HealthComponent?.TakeFixedDamage( 1 );
		Owner?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.PowerUp, 2 );
	}
}
