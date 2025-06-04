namespace CardGame.Effects;

public class EnchantNextTurn( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "Become Enchanted next turn";

	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.StatusEffects?.AddStatusEffectByKeyNextTurn( StatusEffects.StatusEffect.StatusKey.Enchanted );
		base.OnPlay( detail );
	}
}
