﻿namespace CardGame.StatusEffects;

public class Protection( Data.StatusEffect data ) : StatusEffect( data )
{
	public override StatusKey Keyword => StatusKey.Protection;
	
	public override string Description()
	{
		return Stack > 0 ? $"Physical Damage -{Stack} for this turn." : base.Description();
	}

	public override int DamageModifier( Card card, int damage )
	{
		return -Stack;
	}
	
	public override void OnTurnEnd()
	{
		Destroy();
		base.OnTurnEnd();
	}
}
