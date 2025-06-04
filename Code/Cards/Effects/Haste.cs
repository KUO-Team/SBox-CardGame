namespace CardGame.Effects;

public class Haste( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "Gain @ Haste";

	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Haste, Power );
		base.OnPlay( detail );
	}
}
