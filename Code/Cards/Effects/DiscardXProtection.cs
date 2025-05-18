namespace CardGame.Effects;

public class DiscardXProtection( Card card ) : CardEffect( card )
{
	public override string Description => Power.Min > 0 ? "Discard up to @ cards, gain 1 Protection for each card discarded this way" : "Discard a card, gain 1 Protection for each card discarded this way";

	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.HandComponent?.EnterDiscardMode( Card );
		base.OnPlay( detail );
	}

	public override void OnDiscardModeCardDiscard( CardEffectDetail detail, Card card )
	{
		detail.Unit?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Protection );
		base.OnDiscardModeCardDiscard( detail, card );
	}
}
