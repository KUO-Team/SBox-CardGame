namespace CardGame.Effects;

public class DiscardHealth( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "If this card is discarded, recover @ HP";

	public override void OnDiscard( CardEffectDetail detail )
	{
		detail.Unit?.HealthComponent?.Heal( EffectivePower );
		base.OnDiscard( detail );
	}
}
