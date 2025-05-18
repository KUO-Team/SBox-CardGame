namespace CardGame.StatusEffects;

public class Fragile( Data.StatusEffect data ) : StatusEffect( data )
{
	public override StatusKey Keyword => StatusKey.Fragile;
	
	private string GetDescription()
	{
		return Stack > 0 ? $"Take {Stack} additional damage from attacks for this turn." : "Take X additional damage from attacks for this turn.";
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
