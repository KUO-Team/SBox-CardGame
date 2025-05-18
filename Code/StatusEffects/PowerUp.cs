namespace CardGame.StatusEffects;

public class PowerUp( Data.StatusEffect data ) : StatusEffect( data )
{
	public override StatusKey Keyword => StatusKey.PowerUp;
	
	public override string Description()
	{
		return Stack > 0 ? $"Attack Power +{Stack} for this turn." : base.Description();
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
