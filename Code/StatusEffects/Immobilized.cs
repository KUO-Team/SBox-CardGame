namespace CardGame.StatusEffects;

public class Immobilized( Data.StatusEffect data ) : StatusEffect( data )
{
	public override StatusKey Keyword => StatusKey.Immobilized;
	
	public override string Description()
	{
		return Stack > 1 ? $"All card slots become unavailable for {Stack} turns." : base.Description();
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
