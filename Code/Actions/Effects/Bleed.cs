namespace CardGame.Effects;

public class Bleed( Card card, Action action, RangedInt power ) : ActionEffect( card, action, power )
{
	public override string Description => "[On Hit] Inflict @ Bleed";

	public override void OnHit( ActionEffectDetail detail )
	{
		detail.Target?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Bleed, EffectivePower );
		base.OnHit( detail );
	}
}
