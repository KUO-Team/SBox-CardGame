using CardGame.Data;
namespace CardGame.Passives;

public class Solo : PassiveAbility
{
	public override void OnBattleStart( Battle battle )
	{
		if ( !Owner.IsValid() || !Player.Local.IsValid() )
		{
			return;
		}

		if ( ShouldAdd( battle ) )
		{
			var t = Player.Local.Units.FirstOrDefault();
			if ( t.IsValid() )
			{
				Owner.Slots?.AddCardSlot();
			}
		}

		base.OnBattleStart( battle );
	}

	private static bool ShouldAdd( Battle battle )
	{
		if ( battle.Id == 1 )
		{
			return false;
		}
			
		return BattleManager.GetAliveUnits( Faction.Player ).Count < 2;
	}
}
