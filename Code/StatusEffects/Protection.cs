namespace CardGame.StatusEffects;

public class Protection( Data.StatusEffect data ) : StatusEffect( data )
{
	public override StatusKey Keyword => StatusKey.Protection;
	
	public override string Description()
	{
		return $"Take {Stack} less physical damage for this turn.";
	}

	public override int DamageModifier( int damage, Card? card )
	{
		return -Stack;
	}
	
	public override void OnTurnEnd()
	{
		Destroy();
		base.OnTurnEnd();
	}
}
