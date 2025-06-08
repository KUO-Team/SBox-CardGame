using System;
using CardGame.StatusEffects;

namespace CardGame.Relics;

public class BleedingStatue( Data.Relic data ) : Relic( data )
{
	public override void OnTurnStart()
	{
		if ( !BattleManager.Instance.IsValid() )
		{
			return;
		}
		
		var turnCount = BattleManager.Instance.Turn;
		var amount = Math.Min( turnCount, 5 );
		
		foreach ( var unit in BattleManager.AliveUnits )
		{
			if ( !unit.HealthComponent.IsValid() || unit.HealthComponent.IsDead )
			{
				return;
			}
			
			if ( !unit.StatusEffects.IsValid() )
			{
				continue;
			}
			
			unit.StatusEffects.AddStatusEffectByKey( StatusEffect.StatusKey.Bleed, amount );

			if ( unit.Faction != Faction.Player )
			{
				continue;
			}

			var bleed = unit.StatusEffects.FirstOrDefault( x => x is Bleed );
			if ( bleed is null )
			{
				continue;
			}
			
			if ( bleed.Stack >= 5 )
			{
				unit.StatusEffects.AddStatusEffectByKey( StatusEffect.StatusKey.AttackPowerUp, 2 );
			}

			if ( bleed.Stack >= 10 )
			{
				unit.HealthComponent.TakeFixedDamage( 5 );
			}
		}
		
		base.OnTurnStart();
	}
}
