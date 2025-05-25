namespace CardGame.StatusEffects;

public class Silenced( Data.StatusEffect data ) : StatusEffect( data )
{
	public override StatusKey Keyword => StatusKey.Silenced;

	private CardSlot? _slot;

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
		if ( _slot is not null )
		{
			_slot.IsAvailable = true;
			_slot = null;
		}

		base.Destroy();
	}
	
	private void Activate()
	{
		if ( !Owner.IsValid() || !Owner.Slots.IsValid() || Owner.Slots.Count == 0 )
		{
			return;
		}

		_slot = Owner.Slots.FirstOrDefault( slot => slot.IsAvailable );
		if ( _slot is not null )
		{
			_slot.IsAvailable = false;
		}
	}
}
