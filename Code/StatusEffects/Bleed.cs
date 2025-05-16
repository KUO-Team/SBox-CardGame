using CardGame.Data;

namespace CardGame.StatusEffects;

public class Bleed : StatusEffect
{
	public override Id Id => 6;

	public override string Icon => "/Materials/Statuses/Bleed.png";

	public override string Name => "Bleed";

	public override string Description => GetDescription();

	public override bool IsNegative => true;

	public override StatusKey Keyword => StatusKey.Bleed;

	private string GetDescription()
	{
		return Stack > 0 ? $"Upon playing an attack card, take {Stack} damage, then halve the stack." : "Upon playing an attack card, take X damage, then halve the stack.";
	}

	public override void OnPlayCard( Card card )
	{
		if ( card.Type != Card.CardType.Attack )
		{
			return;
		}

		Owner?.HealthComponent?.TakeFixedDamage( Stack );
		
		Stack /= 2;
		if ( Stack <= 0 )
		{
			Destroy();
		}

		base.OnPlayCard( card );
	}
}
