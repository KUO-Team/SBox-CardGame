using CardGame.Data;

namespace CardGame.Events;

public class WeepingStatue( Data.Event data ) : Event( data )
{
	private const int RelicId = 100;
	
	public override void OnShow( Data.Event @event )
	{
		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}

		var unit = player.Unit;
		if ( unit is null )
		{
			return;
		}

		var takeChoice = @event.Choices[0];
		if ( unit.Hp <= 10 )
		{
			takeChoice.Enabled = false;
		}

		base.OnShow( @event );
	}

	public override void OnChoiceSelected( Data.Event.Choice choice, int index )
	{
		var player = Player.Local;
		if ( !player.IsValid() )
		{
			return;
		}

		if ( player.Unit is null )
		{
			return;
		}
		
		switch ( index )
		{
			case 0:
				{
					Player.Local?.Unit?.Damage( 10 );
					EventUtility.AddRelic( RelicId );
				}
				break;
			case 1:
				break;
		}

		base.OnChoiceSelected( choice, index );
	}
}
