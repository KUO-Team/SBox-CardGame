using CardGame.StatusEffects;
using CardGame.Units;

namespace CardGame.Passives;

public class BloodFeeding( Data.PassiveAbility data ) : PassiveAbility( data )
{
	public bool Triggered { get; private set; }
	public int AmountHealed { get; private set; }
	public int LastTurnAmountHealed { get; private set; }

	public override void OnDealDamage( int damage, BattleUnit target )
	{
		if ( !Owner.IsValid() || !Owner.HealthComponent.IsValid() || !target.StatusEffects.IsValid() || Triggered || damage < 1 )
		{
			return;
		}

		if ( target.StatusEffects.HasStatusEffect<Bleed>() )
		{
			AmountHealed += 2;
			Triggered = true;
			Owner.HealthComponent.Heal( 2 );
		}

		base.OnDealDamage( damage, target );
	}

	public override void OnTurnEnd()
	{
		LastTurnAmountHealed = AmountHealed;
		Triggered = false;
		AmountHealed = 0;

		base.OnTurnEnd();
	}
}
