namespace CardGame.Effects;

public class DiscardXProtection( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => PowerRange.Max == 1 ? "Discard a card, gain 1 Protection for each card discarded this way" : "Discard up to @ cards, gain 1 Protection for each card discarded this way";

	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.HandComponent?.EnterDiscardMode( Card, EffectivePower );
		base.OnPlay( detail );
	}

	public override void OnDiscardModeCardDiscard( CardEffectDetail detail, Card card )
	{
		detail.Unit?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Protection );
		base.OnDiscardModeCardDiscard( detail, card );
	}
}
