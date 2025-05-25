namespace CardGame.Effects;

public class Stunned( Card card ) : CardEffect( card )
{
	public override string Description => "Become Stunned";

	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Stunned );
		base.OnPlay( detail );
	}
}
