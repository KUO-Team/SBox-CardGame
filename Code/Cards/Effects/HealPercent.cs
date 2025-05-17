namespace CardGame.Effects;

public class HealPercent( Card card ) : CardEffect( card )
{
	public override string Description { get; set; } = "Heal by @% of max HP";

	public override void OnPlay( CardEffectDetail detail )
	{
		if ( !detail.Unit.IsValid() || !detail.Unit.HealthComponent.IsValid() )
		{
			return;
		}

		var amount = GetPercentageOfHealth( Power.Value / 100f, detail.Unit.HealthComponent.MaxHealth );
		detail.Unit.HealthComponent.Heal( amount );

		base.OnPlay( detail );
	}

	private static int GetPercentageOfHealth( float percentage, int maxHealth )
	{
		return (int)(percentage * maxHealth);
	}
}
