namespace CardGame.Passives;

public class BulkyArmor( Data.PassiveAbility data ) : PassiveAbility( data )
{
	public override void OnTurnStart()
	{
		Owner?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Protection, 2 );
		base.OnTurnStart();
	}
}
