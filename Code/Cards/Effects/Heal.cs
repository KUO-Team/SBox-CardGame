namespace CardGame.Effects;

public class Heal( Card card ) : CardEffect( card )
{
	public override string Description { get; set; } = "Heal @ HP";
	
	public override void OnPlay( CardEffectDetail detail )
	{
		detail.Unit?.HealthComponent?.Heal( Power.Value );
		base.OnPlay( detail );
	}
}
