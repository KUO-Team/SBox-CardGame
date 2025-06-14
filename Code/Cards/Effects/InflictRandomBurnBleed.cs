﻿namespace CardGame.Effects;

public class InflictRandomBurnBleed( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "Randomly inflict @ of the following: Bleed, Burn";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		var t = Game.Random.Float();
		if ( t > 0.5 )
		{
			detail.Target?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Burn, EffectivePower );
		}
		else
		{
			detail.Target?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Bleed, EffectivePower );
		}
		base.OnPlay( detail );
	}
}
