namespace CardGame.Effects;

public class EnchantedOnly( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "Only playable if the owner is Enchanted";

	public override bool CanPlay( CardEffectDetail detail )
	{
		return detail.Unit?.StatusEffects?.HasStatusEffect<StatusEffects.Enchanted>() ?? true;
	}
}
