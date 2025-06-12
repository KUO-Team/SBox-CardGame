namespace CardGame.Effects;

public class DiscardLowestCost( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "Discard the lowest costing card in hand";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		var card = GetLowestCostingCardInHand( detail );
		if ( card is not null )
		{
			detail.Unit?.HandComponent?.Discard( card );
		}

		base.OnPlay( detail );
	}

	private static Card? GetLowestCostingCardInHand( CardEffectDetail detail )
	{
		if ( !detail.Unit.IsValid() || !detail.Unit.HandComponent.IsValid() )
		{
			return null;
		}

		return detail.Unit.HandComponent.Hand
			.OrderBy( x => x.EffectiveCost )
			.FirstOrDefault();
	}
}
