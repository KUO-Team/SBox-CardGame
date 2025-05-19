using Sandbox.UI;
using Sandbox.Diagnostics;
using CardGame.Data;

namespace CardGame.UI.Map;

public partial class EventPanel
{
	public StartingEvent? RootEvent { get; private set; }

	public Event? Event { get; set; }

	public Events.Event? EventScript { get; set; }

	private static readonly Logger Log = new( "Event" );

	public void Show( Event @event )
	{
		Event = @event;

		if ( @event is StartingEvent startingEvent )
		{
			RootEvent = startingEvent;
			LoadEventScript( startingEvent );
		}

		EventScript?.OnShow( @event );
		this.Show();
	}

	public void ShowEventById( Id id )
	{
		var @event = EventDataList.GetById( id );
		if ( @event is not null )
		{
			Show( @event.DeepCopy() );
		}
		else
		{
			Log.Warning( $"Unable to show event with id: {id}; none found!" );
		}
	}

	public void SelectChoice( Event.Choice choice, int index )
	{
		EventScript?.OnChoiceSelected( choice, index );
		this.Hide();

		if ( !choice.HasEvent )
		{
			return;
		}

		var @event = choice.Event;
		if ( @event is null )
		{
			Log.Warning( $"Choice has further event, but event is null!" );
			return;
		}

		Show( @event );
	}

	private void LoadEventScript( StartingEvent @event )
	{
		if ( string.IsNullOrEmpty( @event.Script ) )
		{
			return;
		}

		EventScript = TypeLibrary.Create<Events.Event>( @event.Script, [@event] );
		EventScript.Panel = this;
	}

	private void UnloadEventScript()
	{
		if ( EventScript is null )
		{
			return;
		}

		EventScript = null;
	}
}
