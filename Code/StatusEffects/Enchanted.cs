using CardGame.Data;

namespace CardGame.StatusEffects;

public class Enchanted : StatusEffect
{
	public override Id Id => 9;
	
	public override string Icon => "/Materials/Statuses/Enchanted.png";

	public override string Name => "Enchanted";

	public override string Description => "Used for certain card effects.";

	public override bool IsHiddenStack => true;

	public override int? Maximum => 1;
	
	public override StatusKey Keyword => StatusKey.Enchanted;	
	
	public override void OnTurnEnd()
	{
		Destroy();
		base.OnTurnEnd();
	}
}
