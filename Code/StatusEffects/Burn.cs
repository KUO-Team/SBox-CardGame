namespace CardGame.StatusEffects;

public class Burn( Data.StatusEffect data ) : StatusEffect( data )
{
	public override StatusKey Keyword => StatusKey.Burn;

	public override string Description()
	{
		return Stack > 0 ? $"At the end of the turn, take {Stack} damage, then halve the stack." : base.Description();
	}

	public override void OnTurnEnd()
	{
		Owner?.HealthComponent?.TakeFixedDamage( Stack );

		Stack /= 2;
		if ( Stack <= 0 )
		{
			Destroy();
		}

		base.OnTurnEnd();
	}
}
