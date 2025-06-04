namespace CardGame.Effects;

public class Burn( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "Inflict @ Burn";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Target?.StatusEffects?.AddStatusEffectByKey( StatusEffects.StatusEffect.StatusKey.Burn, Power );
		base.OnPlay( detail );
	}
}
