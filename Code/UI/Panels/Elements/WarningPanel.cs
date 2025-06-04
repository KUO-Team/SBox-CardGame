using Sandbox.UI;
using Sandbox.UI.Construct;

namespace CardGame.UI;

public class WarningPanel : Panel
{
	public List<Button> Buttons { get; private set; } = [];

	public System.Action? OnClose { get; set; } = null;

	private readonly Panel _buttonContainer;

	private Panel? _main;
	private Label? _titleLabel;
	private Label? _warningLabel;

	public WarningPanel( string title, string warning, List<Button>? buttons = null )
	{
		_main = Add.Panel( "main" );
		_titleLabel = _main.Add.Label( title, "title" );
		_titleLabel.IsRich = true;
		_warningLabel = _main.Add.Label( warning, "warning" );
		_warningLabel.IsRich = true;

		_buttonContainer = _main.Add.Panel( "buttons" );
		if ( buttons != null )
		{
			InitializeButtons( buttons );
		}
	}

	public void SetButtons( List<Button> buttons )
	{
		Buttons = buttons;
		_buttonContainer.DeleteChildren();
		AddButtonsToContainer( Buttons );
	}

	public void Close()
	{
		OnClose?.Invoke();
		Delete();
	}

	public static WarningPanel Create( string titleText, string warningText, List<Button>? buttons = null )
	{
		var warning = new WarningPanel( titleText, warningText, buttons );
		Hud.Instance?.AddElement( warning );
		return warning;
	}

	private void InitializeButtons( List<Button> buttons )
	{
		Buttons = buttons;

		if ( Buttons.Count == 0 )
		{
			var closeButton = new Button( "Close", "", Close );
			_buttonContainer.AddChild( closeButton );
		}
		else
		{
			AddButtonsToContainer( Buttons );
		}
	}

	private void AddButtonsToContainer( List<Button> buttons )
	{
		foreach ( var button in buttons )
		{
			_buttonContainer.AddChild( button );
		}
	}
}
