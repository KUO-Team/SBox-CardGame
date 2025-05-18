namespace CardGame.Effects;

public class DiscardPower( Card card ) : CardEffect( card )
{
	public override string Description => Power.Min > 1 ? "If this card is discarded, gain @ Power Up" : "If this card is discarded, gain 1 Power Up";

	public override void OnDiscard( CardEffectDetail detail )
	{
		detail.Unit?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.PowerUp );
		base.OnDiscard( detail );
	}
}
