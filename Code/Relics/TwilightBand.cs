using System;
using CardGame.Data;

namespace CardGame.Relics;

public class TwilightBand( Data.Relic data ) : Relic( data )
{
	public override void OnAdd()
	{
		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}

		if ( player.Unit is null )
		{
			return;
		}

		var newMaxHp = Math.Max( GetHp( player.Unit ), 1 );
		player.Unit.Hp = Math.Min( player.Unit.Hp, newMaxHp );
		player.Unit.MaxHp = newMaxHp;

		base.OnAdd();
	}

	public override void OnBattleStart( Battle battle )
	{
		if ( !Owner.IsValid() || !Owner.Slots.IsValid() )
		{
			return;
		}

		Owner.Slots.AddCardSlot();
		base.OnBattleStart( battle );
	}

	public override void OnTurnStart()
	{
		Owner?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Fragile );
		base.OnTurnStart();
	}

	private static int GetHp( UnitData unit )
	{
		return (int)(unit.MaxHp * 0.85f);
	}
}
