namespace CardGame.StatusEffects;

public class Fragile( Data.StatusEffect data ) : StatusEffect( data )
{
	public override StatusKey Keyword => StatusKey.Fragile;

	public override string Description()
	{
		return $"Take {Stack} additional physical damage for this turn.";
	}

	public override int DamageModifier( Card card, int damage )
	{
		return Stack;
	}
	
	public override void OnTurnEnd()
	{
		Destroy();
		base.OnTurnEnd();
	}
}
