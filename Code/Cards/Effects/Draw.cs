namespace CardGame.Effects;

public class Draw( Card card ) : CardEffect( card )
{
	public override string Description => Power == 1 ? "Draw 1 Card" : "Draw @ Cards";

	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.HandComponent?.DrawX( Power );
		base.OnPlay( detail );
	}
}
