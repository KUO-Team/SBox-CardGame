using CardGame.Data;

namespace CardGame.StatusEffects;

public class PowerUp : StatusEffect
{
	public override Id Id => 1;

	public override string Icon => "/Materials/Statuses/PowerUp.png";

	public override string Name => "Power Up";

	public override string Description => GetDescription();

	public override StatusKey Keyword => StatusKey.PowerUp;
	
	private string GetDescription()
	{
		return Stack > 0 ? $"Attack Power +{Stack} for this turn." : "Attack Power +X for this turn.";
	}

	public override int PowerModifier( Card card, Action action )
	{
		if ( action.Type != Action.ActionType.Attack )
		{
			return 0;
		}
		
		return Stack;
	}
	
	public override void OnTurnEnd()
	{
		Destroy();
		base.OnTurnEnd();
	}
}
