using System;
using CardGame.Data;

namespace CardGame.StatusEffects;

public class Cold : StatusEffect
{
	public override Id Id => 10;
	
	public override string Icon => "/Materials/Statuses/PowerDown.png";

	public override string Name => "Cold";

	public override string Description => GetDescription();

	public override bool IsNegative => true;

	public override StatusKey Keyword => StatusKey.Cold;
	
	private string GetDescription()
	{
		return Stack > 0 ? $"Speed -{Stack} for this turn." : "Speed -X for this turn.";
	}
	
	public override void OnTurnStart()
	{
		if ( !Owner.IsValid() || !Owner.Slots.IsValid() )
		{
			return;
		}

		foreach ( var slot in Owner.Slots )
		{
			slot.Speed = Math.Max( slot.Speed - Stack, 1 );
		}
		
		base.OnTurnStart();
	}
}
