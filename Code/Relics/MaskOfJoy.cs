using System;
using CardGame.Units;

namespace CardGame.Relics;

public class MaskOfJoy( Data.Relic data ) : Relic( data )
{
	private int _damageTaken;

	public override void OnTakeDamage( int damage, BattleUnit target, BattleUnit? attacker = null )
	{
		if ( target != Owner )
		{
			return;
		}

		_damageTaken += damage;
		base.OnTakeDamage( damage, target, attacker );
	}

	public override void OnTurnStart()
	{
		if ( BattleManager.Instance?.Turn == 1 )
		{
			return;
		}
		
		if ( !Owner.IsValid() || !Owner.StatusEffects.IsValid() )
		{
			return;
		}
		
		var amount = _damageTaken / 5;
		if ( amount > 0 )
		{
			var max = Math.Min( amount, 3 );
			Owner.StatusEffects.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.PowerUp, max );
		}

		if ( _damageTaken >= 10 )
		{
			Owner.Mana += 1;
		}

		if ( _damageTaken < 5 )
		{
			Owner.StatusEffects.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.PowerDown, 1 );
		}

		_damageTaken = 0;
		base.OnTurnStart();
	}
}
