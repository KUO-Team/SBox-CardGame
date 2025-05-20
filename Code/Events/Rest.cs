namespace CardGame.Events;

public class Rest( Data.Event data ) : Event( data )
{
	public override void OnChoiceSelected( Data.Event.Choice choice, int index )
	{
		switch ( index )
		{
			case 0:
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

					EventUtility.HealToMax();
				}
				break;
		}
		
		base.OnChoiceSelected( choice, index );
	}
}
