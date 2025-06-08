namespace CardGame.Relics;

public class HexboundCatalyst( Data.Relic data ) : Relic( data )
{
	public override void OnTurnStart()
	{
		foreach ( var unit in BattleManager.GetAliveUnits( Faction.Player ) )
		{
			if ( !unit.StatusEffects.IsValid() )
			{
				continue;
			}

			if ( unit.StatusEffects.HasStatusEffect<StatusEffects.Enchanted>() )
			{
				unit.StatusEffects.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.AttackPowerUp, 2 );
			}
			else
			{
				unit.StatusEffects.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.AttackPowerDown );
			}
		}
		
		base.OnTurnStart();
	}
}
