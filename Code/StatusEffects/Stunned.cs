namespace CardGame.StatusEffects;

public class Stunned( Data.StatusEffect data ) : StatusEffect( data )
{
	public override StatusKey Keyword => StatusKey.Stunned;
	
	public override string Description()
	{
		return Stack > 1 ? $"All card slots become unavailable for {Stack} turns." : "All card slots become unavailable for this turn.";
	}

	public override void OnAddOrUpdate()
	{
		Activate();
		base.OnAddOrUpdate();
	}

	public override void OnTurnStart()
	{
		Activate();
		base.OnTurnStart();
	}

	public override void OnTurnEnd()
	{
		Stack--;
		if ( Stack <= 0 )
		{
			Destroy();
		}
		
		base.OnTurnEnd();
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
	
	private void Activate()
	{
		if ( !Owner.IsValid() || !Owner.Slots.IsValid() )
		{
			return;
		}

		foreach ( var slot in Owner.Slots )
		{
			slot.IsAvailable = false;
		}
	}
}
