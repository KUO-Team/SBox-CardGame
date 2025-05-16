using CardGame.Data;

namespace CardGame.StatusEffects;

public class Burn : StatusEffect
{
	public override Id Id => 5;
	
	public override string Icon => "/Materials/Statuses/Burn.png";

	public override string Name => "Burn";

	public override string Description => GetDescription();

	public override bool IsNegative => true;

	public override StatusKey Keyword => StatusKey.Burn;
	
	private string GetDescription()
	{
		return Stack > 0 ? $"At the end of the turn, take {Stack} damage, then halve the stack." : "At the end of the turn, take X damage, then halve the stack.";
	}

	public override void OnTurnEnd()
	{
		Owner?.HealthComponent?.TakeFixedDamage( Stack );

		Stack /= 2;
		if ( Stack <= 0 )
		{
			Destroy();
		}

		base.OnTurnEnd();
	}
}
