using CardGame.Data;

namespace CardGame.StatusEffects;

public class Silenced : StatusEffect
{
	public override Id Id => 8;
	
	public override string Icon => "/Materials/Statuses/Immobilized.png";

	public override string Name => "Silenced";

	public override string Description => "The left-most available card slot becomes unavailable.";

	public override StatusKey Keyword => StatusKey.Silenced;

	private CardSlot? _slot;

	public override void OnTurnStart()
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

		base.OnTurnStart();
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
}
