namespace CardGame.Effects;

public class DiscardDraw( Card card ) : CardEffect( card )
{
	public override string Description => Power == 1 ? "If this card is discarded, draw 1 card" : "If this card is discarded, draw @ cards";

	public override void OnDiscard( CardEffectDetail detail )
	{
		detail.Unit?.HandComponent?.DrawX( Power );
		base.OnDiscard( detail );
	}
}
