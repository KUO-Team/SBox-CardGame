namespace CardGame.Data;

public class Event : IDeepCopyable<Event>
{
	[TextArea]
	public string Text { get; set; } = string.Empty;

	[InlineEditor, WideMode]
	public List<Choice> Choices { get; set; } = [];

	public class Choice : IDeepCopyable<Choice>
	{
		public string Text { get; set; } = string.Empty;

		public bool Enabled { get; set; } = true;

		public bool HasEvent { get; set; }

		[ShowIf( nameof( HasEvent ), true ), InlineEditor]
		public Event? Event { get; set; }

		public Choice DeepCopy()
		{
			return new Choice
			{
				Text = Text, 
				Enabled = Enabled, 
				HasEvent = HasEvent, 
				Event = Event?.DeepCopy()
			};
		}
	}

	public virtual Event DeepCopy()
	{
		return new Event
		{
			Text = Text, 
			Choices = Choices.Select( c => c.DeepCopy() ).ToList()
		};
	}
}

public class StartingEvent : Event, IResource, IDeepCopyable<StartingEvent>
{
	[InlineEditor, Order( -1 )]
	public Id Id { get; set; } = Id.Invalid;

	public string Script { get; set; } = string.Empty;

	public override StartingEvent DeepCopy()
	{
		return new StartingEvent
		{
			Id = Id, 
			Text = Text, 
			Choices = Choices.Select( c => c.DeepCopy() ).ToList(), 
			Script = Script
		};
	}
}
