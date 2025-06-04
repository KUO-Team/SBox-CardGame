namespace CardGame.Effects;

public class Draw( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => PowerRange.Max == 1 ? "Draw 1 Card" : "Draw @ Cards";

	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.HandComponent?.DrawX( Power );
		base.OnPlay( detail );
	}
}
