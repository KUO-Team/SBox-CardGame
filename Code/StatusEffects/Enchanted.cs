namespace CardGame.StatusEffects;

public class Enchanted( Data.StatusEffect data ) : StatusEffect( data )
{
	public override StatusKey Keyword => StatusKey.Enchanted;	
	
	public override void OnTurnEnd()
	{
		Destroy();
		base.OnTurnEnd();
	}
}
