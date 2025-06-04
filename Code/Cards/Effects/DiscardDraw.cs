namespace CardGame.Effects;

public class DiscardDraw( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => PowerRange.Max == 1 ? "If this card is discarded, draw 1 card" : "If this card is discarded, draw @ cards";

	public override void OnDiscard( CardEffectDetail detail )
	{
		detail.Unit?.HandComponent?.DrawX( Power );
		base.OnDiscard( detail );
	}
}
