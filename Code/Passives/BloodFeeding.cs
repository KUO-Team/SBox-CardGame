using CardGame.StatusEffects;
using CardGame.Units;

namespace CardGame.Passives;

public class BloodFeeding( Data.PassiveAbility data ) : PassiveAbility( data )
{
	public override void OnDealDamage( BattleUnit target )
	{
		if ( target.StatusEffects.IsValid() && target.StatusEffects.HasStatusEffect<Bleed>() )
		{
			Owner?.HealthComponent?.Heal( 2 );
		}
		
		base.OnDealDamage( target );
	}
}
