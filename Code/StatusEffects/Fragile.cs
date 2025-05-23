﻿namespace CardGame.StatusEffects;

public class Fragile( Data.StatusEffect data ) : StatusEffect( data )
{
	public override StatusKey Keyword => StatusKey.Fragile;

	public override string Description()
	{
		return Stack > 0 ? $"Take {Stack} additional damage from attacks for this turn." : base.Description();
	}

	public override int DamageModifier( Card card, int damage )
	{
		return Stack;
	}
	
	public override void OnTurnEnd()
	{
		Destroy();
		base.OnTurnEnd();
	}
}
