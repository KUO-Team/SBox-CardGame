using System;

namespace CardGame.StatusEffects;

public class ManaDown( Data.StatusEffect data ) : StatusEffect( data )
{
	public override StatusKey Keyword => StatusKey.ManaDown;
	
	public override string Description()
	{
		return Stack > 0 ? $"At the start of the turn, remove {Stack} MP." : base.Description();
	}

	public override void OnTurnStart()
	{
		if ( !Owner.IsValid() )
		{
			return;
		}

		Owner.Mana = Math.Max( Owner.Mana -= Stack, 0 );
		base.OnTurnStart();
	}

	public override void OnTurnEnd()
	{
		Destroy();
		base.OnTurnEnd();
	}
}
