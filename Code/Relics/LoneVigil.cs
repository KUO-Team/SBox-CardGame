using System;

namespace CardGame.Relics;

public class LoneVigil( Data.Relic data ) : Relic( data )
{
	public override void OnTurnStart()
	{
		if ( !Owner.IsValid() || !Owner.HealthComponent.IsValid() || Owner.HealthComponent.IsDead )
		{
			return;
		}

		var allies = BattleManager.GetAliveUnits( Owner.Faction );
		var enemies = BattleManager.GetAliveUnits( Owner.Faction.GetOpposite() );
		var difference = enemies.Count - allies.Count;

		switch ( difference )
		{
			case > 0:
				{
					// More enemies than allies: gain 1 Power Up for each extra enemy
					for ( var i = 0; i < difference; i++ )
					{
						Owner.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.AttackPowerUp );
					}
					break;
				}
			case < 0:
				{
					// More allies than enemies: randomly pick an ally to get Fragile for each excess ally
					var aliveAllies = allies.Where( unit => unit.IsValid() ).ToList();
					var fragileCount = Math.Abs( difference );

					for ( var i = 0; i < fragileCount; i++ )
					{
						if ( aliveAllies.Count == 0 )
						{
							break;
						}

						var randomAlly = aliveAllies[Game.Random.Next( 0, aliveAllies.Count )];
						randomAlly.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Fragile );
					}
					break;
				}
		}

		base.OnTurnStart();
	}
}
