namespace CardGame.Effects;

public class ExhaustThis( Card card ) : CardEffect( card )
{
	public override string Description => "Exhaust this card";

	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.HandComponent?.Exhaust( Card );
	}
}
