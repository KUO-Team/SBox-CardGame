namespace CardGame.Effects;

public class Draw( Card card ) : CardEffect( card )
{
	public override string Description => Power.Max > 0 ? "Draw @ Cards" : "Draw 1 Card";

	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.HandComponent?.DrawX( Power.Value );
		base.OnPlay( detail );
	}
}
