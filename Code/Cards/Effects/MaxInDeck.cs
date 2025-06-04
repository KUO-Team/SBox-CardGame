namespace CardGame.Effects;

public class MaxInDeck( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override bool CanAddToDeck( CardEffectDetail detail )
	{
		return base.CanAddToDeck( detail );
	}
}
