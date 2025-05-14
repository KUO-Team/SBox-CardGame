using System;

namespace CardGame.Relics;

public class PowerRing( Data.Relic data ) : Relic( data )
{
	public override void OnTurnStart()
	{
		if ( BattleManager.Instance.IsValid() )
		{
			var turnCount = BattleManager.Instance.Turn;
			var amount = Math.Min( turnCount, 5 );
			
			Owner?.StatusEffects?.AddStatusEffect<StatusEffects.PowerUp>( amount );
			Owner?.StatusEffects?.AddStatusEffect<StatusEffects.Fragile>( amount );
		}
		
		base.OnTurnStart();
	}
}
