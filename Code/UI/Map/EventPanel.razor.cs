using Sandbox.UI;
using Sandbox.Diagnostics;
using CardGame.Data;

namespace CardGame.UI.Map;

public partial class EventPanel
{
	public Event? Event { get; set; }
	
	private static readonly Logger Log = new( "Event" );

	public void Show( Event @event )
	{
		Event = @event;
		this.Show();
	}

	public void ShowEventById( Id id )
	{
		var @event = EventDataList.GetById( id );
		if ( @event is not null )
		{
			Show( @event );
		}
		else
		{
			Log.Warning( $"Unable to show event with id: {id}; none found!" );
		}
	}

	public void SelectChoice( Event.Choice choice )
	{
		choice.OnSelected?.Invoke();
		Log.Info( $"Selected choice: {choice}" );
		this.Hide();

		if ( choice.HasEvent )
		{
			var @event = choice.Event;
			if ( @event is null )
			{
				Log.Warning( $"Choice has further event, but event is null!" );
				return;
			}
			Show( @event );
		}
	}
}
