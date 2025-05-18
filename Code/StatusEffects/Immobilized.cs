namespace CardGame.StatusEffects;

public class Immobilized( Data.StatusEffect data ) : StatusEffect( data )
{
	public override StatusKey Keyword => StatusKey.Immobilized;
	
	private string GetDescription()
	{
		return Stack > 1 ? $"All card slots become unavailable for {Stack} turns." : "All card slots become unavailable for this turn.";
	}

	public override void OnTurnStart()
	{
		if ( !Owner.IsValid() || !Owner.Slots.IsValid() )
		{
			return;
		}

		foreach ( var slot in Owner.Slots )
		{
			slot.IsAvailable = false;
		}
		
		base.OnTurnStart();
	}

	public override void Destroy()
	{
		if ( !Owner.IsValid() || !Owner.Slots.IsValid() )
		{
			return;
		}

		foreach ( var slot in Owner.Slots )
		{
			slot.IsAvailable = true;
		}
		
		base.Destroy();
	}
}
