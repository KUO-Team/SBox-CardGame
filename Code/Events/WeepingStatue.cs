namespace CardGame.Events;

public class WeepingStatue( Data.Event data ) : Event( data )
{
	private const int RelicId = 100;
	private const int DamageAmount = 10;

	public override void OnShow( Data.Event @event )
	{
		for ( var i = 0; i < @event.Choices.Count; i++ )
		{
			var choice = @event.Choices[i];
			switch ( i )
			{
				case 1:
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

						if ( unit.Hp <= DamageAmount )
						{
							choice.Enabled = false;
						}
						break;
					}
			}
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
				EventUtility.AddRelic( RelicId );
				break;
			case 1:
				EventUtility.TakePlayerDamage( DamageAmount );
				break;
		}

		base.OnChoiceSelected( choice, index );
	}
}
