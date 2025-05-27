namespace CardGame.StatusEffects;

public class ManaUp( Data.StatusEffect data ) : StatusEffect( data )
{
	public override StatusKey Keyword => StatusKey.ManaUp;
	
	public override string Description()
	{
		return $"At the start of the turn, add {Stack} MP.";
	}

	public override void OnTurnStart()
	{
		if ( !Owner.IsValid() )
		{
			return;
		}

		Owner.Mana += Stack;
		base.OnTurnStart();
	}

	public override void OnTurnEnd()
	{
		Destroy();
		base.OnTurnEnd();
	}
}
