namespace CardGame.Effects;

public class DiscardX( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => PowerRange.Max == 1 ? "Discard 1 Card" : "Discard @ Cards";

	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.HandComponent?.EnterDiscardMode( Card, Power );
		base.OnPlay( detail );
	}
}
