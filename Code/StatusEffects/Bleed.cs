namespace CardGame.StatusEffects;

public class Bleed( Data.StatusEffect data ) : StatusEffect( data )
{
	public override StatusKey Keyword => StatusKey.Bleed;

	public override string Description()
	{
		return Stack > 0 ? $"Upon playing an attack card, take {Stack} damage, then halve the stack." : "Upon playing an attack card, take X damage, then halve the stack.";
	}

	public override void OnPlayCard( Card card )
	{
		if ( card.Type != Card.CardType.Attack )
		{
			return;
		}

		Owner?.HealthComponent?.TakeFixedDamage( Stack );
		
		Stack /= 2;
		if ( Stack <= 0 )
		{
			Destroy();
		}

		base.OnPlayCard( card );
	}
}
