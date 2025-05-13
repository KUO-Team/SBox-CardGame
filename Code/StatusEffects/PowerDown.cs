using System;
using CardGame.Data;

namespace CardGame.StatusEffects;

public class PowerDown : StatusEffect
{
	public override Id Id => 2;
	
	public override string Icon => "/Materials/Statuses/PowerDown.png";

	public override string Name => "Power Down";

	public override string Description => GetDescription();

	public override bool IsNegative => true;

	public override StatusKey Keyword => StatusKey.PowerDown;
	
	private string GetDescription()
	{
		return Stack > 0 ? $"Attack Power -{Stack} for this turn." : "Attack Power -X for this turn.";
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
