using System;
using CardGame.Units;

namespace CardGame.Relics;

public class EchoingShield( Data.Relic data ) : Relic( data )
{
	private static int TurnCount => BattleManager.Instance?.Turn ?? 0;

	private int _lastTurnPlayedCards;
	
	public override void OnTurnStart()
	{
		if ( !Owner.IsValid() || !Owner.StatusEffects.IsValid() )
		{
			return;
		}

		if ( TurnCount == 1 )
		{
			return;
		}

		var amount = Math.Min( _lastTurnPlayedCards, 3 );
		Owner.StatusEffects.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Protection, amount );
		_lastTurnPlayedCards = 0;

		base.OnTurnStart();
	}

	public override void OnPlayCard( Card card, BattleUnitComponent unitComponent )
	{
		_lastTurnPlayedCards++;
		base.OnPlayCard( card, unitComponent );
	}
}
