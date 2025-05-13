namespace CardGame.Effects;

public class MaxInDeck( Card card ) : CardEffect( card )
{
	public override bool CanAddToDeck( CardEffectDetail detail )
	{
		return base.CanAddToDeck( detail );
	}
}
