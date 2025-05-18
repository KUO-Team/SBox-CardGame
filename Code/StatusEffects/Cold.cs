using System;

namespace CardGame.StatusEffects;

public class Cold( Data.StatusEffect data ) : StatusEffect( data )
{
	public override StatusKey Keyword => StatusKey.Cold;
	
	private string GetDescription()
	{
		return Stack > 0 ? $"Speed -{Stack} for this turn." : "Speed -X for this turn.";
	}
	
	public override void OnTurnStart()
	{
		if ( !Owner.IsValid() || !Owner.Slots.IsValid() )
		{
			return;
		}

		foreach ( var slot in Owner.Slots )
		{
			slot.Speed = Math.Max( slot.Speed - Stack, 1 );
		}
		
		base.OnTurnStart();
	}
}
