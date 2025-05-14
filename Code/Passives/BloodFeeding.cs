using CardGame.Data;
using CardGame.StatusEffects;
using CardGame.Units;

namespace CardGame.Passives;

public class BloodFeeding : PassiveAbility
{
	public override Id Id => 1;
	public override string Name => "Blood Feeding";
	public override string Description => "On Hit, if target has Bleed, heal 2HP.";

	public override void OnDealDamage( BattleUnit target )
	{
		if ( target.StatusEffects.IsValid() && target.StatusEffects.HasStatusEffect<Bleed>() )
		{
			Owner?.HealthComponent?.Heal( 2 );
		}
		
		base.OnDealDamage( target );
	}
}
