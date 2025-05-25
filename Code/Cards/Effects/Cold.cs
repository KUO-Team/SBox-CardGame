namespace CardGame.Effects;

public class Cold( Card card ) : CardEffect( card )
{
	public override string Description => "Inflict @ Cold";

	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Target?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Cold, Power.Value );
		base.OnPlay( detail );
	}
}
