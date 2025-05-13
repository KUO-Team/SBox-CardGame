namespace CardGame.Effects;

public class PowerInHand( Card card ) : CardEffect( card )
{
	public override string Description => $"Deal damage equal to the amount of cards named {Card.Name} in hand";

	public override int DamageModifier( CardEffectDetail detail )
	{
		return detail.Unit?.HandComponent?.Hand.Count( x => x.Name == Card.Name ) ?? 0;
	}
}
