using CardGame.Data;

namespace CardGame.StatusEffects;

public class Protection : StatusEffect
{
	public override Id Id => 3;
	
	public override string Icon => "/Materials/Statuses/Protection.png";

	public override string Name => "Protection";

	public override string Description => GetDescription();

	public override StatusKey Keyword => StatusKey.Protection;
	
	private string GetDescription()
	{
		return Stack > 0 ? $"Physical Damage -{Stack} for this turn." : "Physical Damage -X for this turn.";
	}

	public override int DamageModifier( Card card, int damage )
	{
		return -Stack;
	}
	
	public override void OnTurnEnd()
	{
		Destroy();
		base.OnTurnEnd();
	}
}
