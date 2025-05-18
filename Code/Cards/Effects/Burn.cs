namespace CardGame.Effects;

public class Burn( Card card ) : CardEffect( card )
{
	public override string Description => "Inflict @ Burn";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Target?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Burn, Power.Value );
		base.OnPlay( detail );
	}
}
