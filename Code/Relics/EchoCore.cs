using System;
using CardGame.Units;
using CardGame.StatusEffects;

namespace CardGame.Relics;

public class EchoCore( Data.Relic data ) : Relic( data )
{
	private int _lastTurnDiscards;
	private int _currentTurnDiscards;

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

		var powerUpCount = Math.Min( _lastTurnDiscards, 3 );
		for ( var i = 0; i < powerUpCount; i++ )
		{
			Owner.StatusEffects.AddStatusEffect<PowerUp>();
		}

		if ( powerUpCount == 0 )
		{
			Owner.StatusEffects.AddStatusEffect<PowerDown>();
		}

		base.OnTurnStart();
	}

	public override void OnTurnEnd()
	{
		_lastTurnDiscards = _currentTurnDiscards;
		_currentTurnDiscards = 0;
		base.OnTurnEnd();
	}

	public override void OnDiscardCard( Card card, BattleUnit unit )
	{
		if ( unit == Owner )
		{
			_currentTurnDiscards++;
		}

		base.OnDiscardCard( card, unit );
	}
}
