﻿using System;

namespace CardGame.StatusEffects;

public class PowerDown( Data.StatusEffect data ) : StatusEffect( data )
{
	public override StatusKey Keyword => StatusKey.PowerDown;
	
	public override string Description()
	{
		return Stack > 0 ? $"Attack Power -{Stack} for this turn." : base.Description();
	}

	public override int PowerModifier( Card card, Action action )
	{
		if ( action.Type != Action.ActionType.Attack )
		{
			return 0;
		}

		var power = action.EffectivePower.Value;
		return Math.Max( power - Stack, 0 );
	}
		
	public override void OnTurnEnd()
	{
		Destroy();
		base.OnTurnEnd();
	}
}
