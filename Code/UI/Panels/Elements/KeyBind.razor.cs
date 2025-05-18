using System;
using Sandbox.UI;

namespace CardGame.UI;

public partial class KeyBind
{
	public InputAction? Action { get; set; }
	
	public Action<PanelEvent>? OnChange { get; set; }
	
	private static bool IsController => Sandbox.Input.UsingController;
	
	private string Input => IGameInstance.Current.GetBind( Action?.Name, out var @default, out var common );
	
	private Label? _label;
	
	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime )
		{
			return;
		}

		AcceptsFocus = true;
		BindClass( "focused", () => HasFocus );

		base.OnAfterTreeRender( firstTime );
	}
	
	public void Apply()
	{
		if ( Action is null )
		{
			return;
		}
		
		IGameInstance.Current.SetBind( Action.Name, Action.KeyboardCode );
		Log.Info( $"Set Action: {Action.Name} to: {Action.KeyboardCode}" );
		InputFocus.Clear( this );
	}
	
	private void Change( ButtonEvent e )
	{
		if ( Action is null )
		{
			return;
		}
		
		if ( !HasFocus )
		{
			return;
		}

		Action.KeyboardCode = e.Button;
		Apply();
	}
	
	public override void OnButtonTyped( ButtonEvent e )
	{
		// We probably shouldn't accept mouse inputs as key binds.
		// Also, OnButtonTyped runs on click, so we need to ignore that.
		var ignoredButtons = new[]
		{
			"mouseleft",
			"mouseright"
		};

		if ( ignoredButtons.Contains( e.Button ) )
			return;

		Change( e );
		base.OnButtonTyped( e );
	}
	
	/// <summary>
	/// Prints out the input bound to the provided action.
	/// </summary>
	/// <param name="action"></param>
	[ConCmd]
	internal static void LogInput( string action )
	{
		var actionBind = IGameInstance.Current.GetBind( action, out var isDefault, out var isCommon );
		Log.Info( $"Bind: {actionBind} : is default: {isDefault} : is common: {isCommon}" );
	}
	
	protected override int BuildHash()
	{
		return HashCode.Combine( HasFocus, Action, Action?.KeyboardCode, Action?.GamepadCode );
	}
}
