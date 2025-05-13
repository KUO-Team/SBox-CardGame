using Sandbox.UI;

namespace CardGame.UI.Map;

public record MapConnection
{
	public int From { get; set; }
	public int To { get; set; }
	public Panel LinePanel { get; set; }

	// ReSharper disable once ConvertToPrimaryConstructor
	public MapConnection( int from, int to, Panel panel )
	{
		From = from;
		To = to;
		LinePanel = panel;
	}
}
