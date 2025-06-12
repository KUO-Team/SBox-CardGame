namespace CardGame.Effects;

public class HealPercent( Card card, RangedInt power ) : CardEffect( card, power )
{
	public override string Description => "Heal by @% of max HP";

	public override void OnPlay( CardEffectDetail detail )
	{
		if ( !detail.Unit.IsValid() || !detail.Unit.HealthComponent.IsValid() )
		{
			return;
		}

		var amount = GetPercentageOfHealth( EffectivePower / 100f, detail.Unit.HealthComponent.MaxHealth );
		detail.Unit.HealthComponent.Heal( amount );

		base.OnPlay( detail );
	}

	private static int GetPercentageOfHealth( float percentage, int maxHealth )
	{
		return (int)(percentage * maxHealth);
	}
}
