namespace CardGame.StatusEffects;

public class Haste( Data.StatusEffect data ) : StatusEffect( data )
{
	public override StatusKey Keyword => StatusKey.Haste;

	public override string Description()
	{
		return Stack > 0 ? $"Speed +{Stack} for this turn." : base.Description();
	}

	public override void OnAdd()
	{
		base.OnAdd();
		Activate();
	}

	public override void OnTurnStart()
	{
		Activate();
		base.OnTurnStart();
	}
	
	public override void OnTurnEnd()
	{
		Destroy();
		base.OnTurnEnd();
	}

	private void Activate()
	{
		if ( !Owner.IsValid() || !Owner.Slots.IsValid() )
		{
			return;
		}

		foreach ( var slot in Owner.Slots )
		{
			slot.Speed = slot.BaseSpeed + Stack;
		}
	}
}
