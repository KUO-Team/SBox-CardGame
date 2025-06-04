namespace CardGame.Effects;

public class ExhaustThis( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "Exhaust this card";

	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.HandComponent?.Exhaust( Card );
	}
}
