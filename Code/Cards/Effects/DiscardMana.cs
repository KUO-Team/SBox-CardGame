namespace CardGame.Effects;

public class DiscardMana( Card card ) : CardEffect( card )
{
	public override string Description => "If this card is discarded, recover @ MP";

	public override void OnDiscard( CardEffectDetail detail )
	{
		detail.Unit?.RecoverMana( Power );
		base.OnDiscard( detail );
	}
}
