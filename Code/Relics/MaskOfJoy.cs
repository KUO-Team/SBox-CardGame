using System;
using CardGame.Units;

namespace CardGame.Relics;

public class MaskOfJoy( Data.Relic data ) : Relic( data )
{
	private int _damageTaken;
	
	private const int MaxBuffAmount = 3;

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
			var max = Math.Min( amount, MaxBuffAmount );
			Owner.StatusEffects.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.PowerUp, max );
		}
		else
		{
			Owner.StatusEffects.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.PowerDown );	
		}

		_damageTaken = 0;
		base.OnTurnStart();
	}
}
