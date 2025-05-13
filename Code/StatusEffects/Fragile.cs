using CardGame.Data;

namespace CardGame.StatusEffects;

public class Fragile : StatusEffect
{
	public override Id Id => 4;
	
	public override string Icon => "/Materials/Statuses/Fragile.png";

	public override string Name => "Fragile";

	public override string Description => GetDescription();

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
