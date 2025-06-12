using CardGame.Modifiers;

namespace CardGame.Effects;

public class LowerCostSameCard( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "Lower the cost of this card by @";

	public override void OnPlay( CardEffectDetail detail )
	{
		Card.Modifiers.AddModifier( new CostModifier( -1, 1 ) );
		base.OnPlay( detail );
	}
}
