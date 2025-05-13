using System;

namespace CardGame.StatusEffects;

public class Cold : StatusEffect
{
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
