namespace CardGame.Effects;

public class DiscardHighestCost( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "Discard the highest costing card in hand";

	public override void OnPlay( CardEffectDetail detail )
	{
		var card = GetHighestCostingCardInHand( detail );
		if ( card is not null )
		{
			detail.Unit?.HandComponent?.Discard( card );
		}

		base.OnPlay( detail );
	}

	private static Card? GetHighestCostingCardInHand( CardEffectDetail detail )
	{
		if ( !detail.Unit.IsValid() || !detail.Unit.HandComponent.IsValid() )
		{
			return null;
		}

		return detail.Unit.HandComponent.Hand
			.OrderByDescending( x => x.EffectiveCost )
			.FirstOrDefault();
	}
}
