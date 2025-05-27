namespace CardGame.Effects;

public class DiscardHealth( Card card ) : CardEffect( card )
{
	public override string Description => "If this card is discarded, recover @ HP";

	public override void OnDiscard( CardEffectDetail detail )
	{
		detail.Unit?.HealthComponent?.Heal( Power );
		base.OnDiscard( detail );
	}
}
