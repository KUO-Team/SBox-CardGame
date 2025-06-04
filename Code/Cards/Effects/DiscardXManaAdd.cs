namespace CardGame.Effects;

public class DiscardXManaAdd( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => PowerRange.Max == 1 ? "Discard a card, add 1 MP for each card discarded this way" : "Discard up to @ cards, add 1 MP for each card discarded this way";

	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.HandComponent?.EnterDiscardMode( Card );
		base.OnPlay( detail );
	}

	public override void OnDiscardModeCardDiscard( CardEffectDetail detail, Card card )
	{
		if ( !detail.Unit.IsValid() )
		{
			return;
		}
		
		detail.Unit.Mana += 1;
		base.OnDiscardModeCardDiscard( detail, card );
	}
}
