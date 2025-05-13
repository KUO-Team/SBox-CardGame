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

			if ( unit.StatusEffects.OfType<StatusEffects.Enchanted>().Any() )
			{
				unit.StatusEffects.AddStatusEffect<StatusEffects.PowerUp>( 2 );
			}
			else
			{
				unit.StatusEffects.AddStatusEffect<StatusEffects.PowerDown>( 2 );
			}
		}
		
		base.OnTurnStart();
	}
}
