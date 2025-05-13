using CardGame.Modifiers;

namespace CardGame.Effects;

public class LowerCostSameCard( Card card ) : CardEffect( card )
{
	public override string Description => "Lower the cost of this card by @";

	public override void OnPlay( CardEffectDetail detail )
	{
		Card.Modifiers.AddModifier( new CostModifier( 0, -1, 1 ) );
		base.OnPlay( detail );
	}
}
