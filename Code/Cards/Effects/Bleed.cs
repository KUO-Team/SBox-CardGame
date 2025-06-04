namespace CardGame.Effects;

public class Bleed( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "Inflict @ Bleed";

	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Target?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Bleed, Power );
		base.OnPlay( detail );
	}
}
