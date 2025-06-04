namespace CardGame.Effects;

public class PowerUpNextTurn( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "Gain @ Power Up next turn";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.StatusEffects?.AddStatusEffectByKeyNextTurn( StatusEffects.StatusEffect.StatusKey.PowerUp, Power );
		base.OnPlay( detail );
	}
}
