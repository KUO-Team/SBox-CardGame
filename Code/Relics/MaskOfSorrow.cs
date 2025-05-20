using System;
using CardGame.Units;

namespace CardGame.Relics;

public class MaskOfSorrow( Data.Relic data ) : Relic( data )
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
		
		if ( _damageTaken <= 0 )
		{
			Owner.SpendMana( 1 );
		}
		
		base.OnTurnStart();
	}
}
