namespace CardGame.Effects;

public class Adrenaline( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "Become Stunned next turn<br>Draw 2 Cards<br>Gain @ Attack Power Up";

	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.StatusEffects?.AddStatusEffectByKeyNextTurn( StatusEffects.StatusEffect.StatusKey.Stunned, 1 );
		detail.Unit?.HandComponent?.DrawX( 2 );
		detail.Unit?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.AttackPowerUp, EffectivePower );
		base.OnPlay( detail );
	}
}
