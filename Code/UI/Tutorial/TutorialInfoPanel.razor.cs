namespace CardGame.UI.Tutorial;

public partial class TutorialInfoPanel( string text, System.Action? onComplete )
{
	public string Text { get; set; } = text;

	public System.Action? OnComplete { get; set; } = onComplete;
}
