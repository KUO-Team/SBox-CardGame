namespace CardGame.Events;

public class WeepingStatue( Data.Event data ) : Event( data )
{
	private const int RelicId = 100;

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
				EventUtility.AddRelic( RelicId );
				break;
			case 1:
				break;
		}

		base.OnChoiceSelected( choice, index );
	}
}
