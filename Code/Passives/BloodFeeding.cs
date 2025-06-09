using System;
using CardGame.StatusEffects;
using CardGame.Units;

namespace CardGame.Passives;

public class BloodFeeding( Data.PassiveAbility data ) : PassiveAbility( data )
{
	public bool Triggered { get; private set; }
	public int AmountHealed { get; private set; }
	public int LastTurnAmountHealed { get; private set; }

	public override void OnDealDamage( int damage, BattleUnitComponent target )
	{
		if ( !Owner.IsValid() || !Owner.HealthComponent.IsValid() || !target.StatusEffects.IsValid() || Triggered || damage < 1 )
		{
			return;
		}

		if ( target.StatusEffects.HasStatusEffect<Bleed>() )
		{
			var cappedDamage = Math.Min( damage, 2 );
			AmountHealed = cappedDamage;
			Triggered = true;
			Owner.HealthComponent.Heal( cappedDamage );
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
