namespace CardGame.Effects;

public class Burn( Card card, Action action, RangedInt power ) : ActionEffect( card, action, power )
{
	public override string Description => "[On Hit] Inflict @ Burn";
	
	public override void OnHit( ActionEffectDetail detail )
	{
		detail.Target?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Burn, EffectivePower );
		base.OnHit( detail );
	}
}
