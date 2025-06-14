﻿using CardGame.Units;

namespace CardGame.Passives;

public class Player( Data.PassiveAbility data ) : PassiveAbility( data )
{
	public override void OnTakeDamage( int damage, BattleUnitComponent attacker )
	{
		if ( !Owner.IsValid() || !Owner.HealthComponent.IsValid() )
		{
			return;
		}

		if ( Owner.HealthComponent.IsDead )
		{
			OnKilledByAttack( attacker );
		}

		base.OnTakeDamage( damage, attacker );
	}

	protected virtual void OnKilledByAttack( BattleUnitComponent attacker )
	{
		GameManager.Instance?.EndRunInLoss();
	}
}
