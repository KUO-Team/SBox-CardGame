namespace CardGame.Effects;

public class DiscardPower( Card card ) : CardEffect( card )
{
	public override string Description => "If this card is discarded, gain @ Power Up";

	public override void OnDiscard( CardEffectDetail detail )
	{
		detail.Unit?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.PowerUp, Power );
		base.OnDiscard( detail );
	}
}
