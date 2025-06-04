namespace CardGame.Effects;

public class Cold( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "Inflict @ Cold";

	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Target?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Cold, Power );
		base.OnPlay( detail );
	}
}
