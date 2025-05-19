using System;

namespace CardGame.StatusEffects;

public class Haste( Data.StatusEffect data ) : StatusEffect( data )
{
	public override StatusKey Keyword => StatusKey.Haste;

	public override string Description()
	{
		return Stack > 0 ? $"Speed +{Stack} for this turn." : base.Description();
	}

	public override void OnTurnStart()
	{
		if ( !Owner.IsValid() || !Owner.Slots.IsValid() )
		{
			return;
		}

		foreach ( var slot in Owner.Slots )
		{
			slot.Speed += Stack;
		}
		
		base.OnTurnStart();
	}
}
