using System;

namespace CardGame.StatusEffects;

public class Cold( Data.StatusEffect data ) : StatusEffect( data )
{
	public override StatusKey Keyword => StatusKey.Cold;

	public override string Description()
	{
		return $"Speed -{Stack} for this turn.";
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
			slot.Speed = Math.Max( slot.Speed - Stack, 1 );
		}
	}
}
