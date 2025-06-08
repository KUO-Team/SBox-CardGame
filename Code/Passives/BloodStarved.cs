using CardGame.StatusEffects;

namespace CardGame.Passives;

public class BloodStarved( Data.PassiveAbility data ) : PassiveAbility( data )
{
	private static int TurnCount => BattleManager.Instance?.Turn ?? 0;

	public override void OnTurnStart()
	{
		if ( !Owner.IsValid() || !Owner.StatusEffects.IsValid() || !Owner.HealthComponent.IsValid() )
		{
			return;
		}

		if ( TurnCount == 1 )
		{
			return;
		}

		var bloodfeeding = GetBloodFeedingPassiveAbility();
		if ( bloodfeeding is null )
		{
			return;
		}

		if ( bloodfeeding.LastTurnAmountHealed > 0 )
		{
			return;
		}

		Owner.StatusEffects.AddStatusEffectByKey( StatusEffect.StatusKey.AttackPowerDown );
		Owner.HealthComponent.TakeFixedDamage( 2 );

		base.OnTurnStart();
	}

	private BloodFeeding? GetBloodFeedingPassiveAbility()
	{
		if ( !Owner.IsValid() )
		{
			return null;
		}

		return !Owner.Passives.IsValid() ? null : Owner.Passives.GetPassiveAbility<BloodFeeding>();
	}
}
