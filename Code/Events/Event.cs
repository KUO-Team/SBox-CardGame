using CardGame.UI.Map;

namespace CardGame.Events;

public abstract class Event( Data.Event data )
{
	public Data.Event Data { get; set; } = data;

	public EventPanel? Panel { get; set; }

	public virtual void OnShow( Data.Event @event )
	{
		
	}

	public virtual void OnSelectChoice( Data.Event.Choice choice )
	{
		
	}
}
