namespace CardGame.Effects;

public class PowerUpNextTurn( Card card ) : CardEffect( card )
{
	public override string Description => "Gain @ Power Up next turn";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.StatusEffects?.AddStatusEffectByKeyNextTurn( StatusEffects.StatusEffect.StatusKey.PowerUp, Power.Value );
		base.OnPlay( detail );
	}
}
