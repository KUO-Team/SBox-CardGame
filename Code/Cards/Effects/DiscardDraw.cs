namespace CardGame.Effects;

public class DiscardDraw( Card card ) : CardEffect( card )
{
	public override string Description => Power > 1 ? "If this card is discarded, draw @ cards" : "If this card is discarded, draw 1 card";

	public override void OnDiscard( CardEffectDetail detail )
	{
		detail.Unit?.HandComponent?.DrawX( Power );
		base.OnDiscard( detail );
	}
}
