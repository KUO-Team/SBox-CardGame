using CardGame.Data;

namespace CardGame.Relics;

public class TwilightBand( Data.Relic data ) : Relic( data )
{
	public override void OnBattleStart( Battle battle )
	{
		if ( !Owner.IsValid() || !Owner.HealthComponent.IsValid() )
		{
			return;
		}

		Owner.Slots?.AddCardSlot();

		var hp = GetHp();
		Owner.HealthComponent.MaxHealth = hp;
		Owner.HealthComponent.Health = Owner.HealthComponent.MaxHealth;

		base.OnBattleStart( battle );
	}

	public override void OnTurnStart()
	{
		Owner?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Fragile );
		base.OnTurnStart();
	}

	private int GetHp()
	{
		if ( !Owner.IsValid() || !Owner.HealthComponent.IsValid() )
		{
			return 0;
		}

		return (int)(Owner.HealthComponent.MaxHealth * 0.85f);
	}
}
