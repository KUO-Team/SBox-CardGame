﻿using CardGame.Data;

namespace CardGame.Relics;

public class MemoryChain( Data.Relic data ) : Relic( data )
{
	public override void OnBattleStart( Battle battle )
	{
		if ( !Owner.IsValid() || !Owner.Slots.IsValid() )
		{
			return;
		}

		Owner.Slots?.AddCardSlot( 2 );
	}

	public override void OnCombatStart()
	{
		if ( !Owner.IsValid() || !Owner.HealthComponent.IsValid() || !Owner.Slots.IsValid() )
		{
			return;
		}

		foreach ( var slot in Owner.Slots )
		{
			if ( slot.IsAssigned )
			{
				continue;
			}
			
			Owner.HealthComponent.TakeFixedDamage( 2 );
		}
	}
}
