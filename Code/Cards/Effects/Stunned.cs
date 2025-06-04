namespace CardGame.Effects;

public class Stunned( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "Become Stunned";

	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Stunned );
		base.OnPlay( detail );
	}
}
