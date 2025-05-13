using System;
using Sandbox.UI;

namespace CardGame.UI.Tutorial;

public partial class TutorialPanel
{
	public TutorialInfoPanel? InfoPanel { get; private set; }
	
	public TutorialDisplayPanel? DisplayPanel { get; private set; }

	protected override void OnStart()
	{
		GameObject.Flags = GameObjectFlags.DontDestroyOnLoad;
		base.OnStart();
	}

	public void ShowInfo( string text, System.Action? onComplete = null, (Length? Left, Length? Top)? displayPosition = null, (Length? Width, Length? Height)? displaySize = null )
	{
		if ( InfoPanel is not null )
		{
			HideInfo( onComplete );
		}

		InfoPanel = new TutorialInfoPanel( text, onComplete );
		Panel.AddChild( InfoPanel );

		if ( DisplayPanel.IsValid() )
		{
			DisplayPanel.Delete();
			DisplayPanel = null;
		}
		
		if ( displayPosition.HasValue || displaySize.HasValue )
		{
			DisplayPanel = new TutorialDisplayPanel();
			Panel.AddChild( DisplayPanel );

			if ( displayPosition.HasValue )
			{
				DisplayPanel.Style.Left = displayPosition.Value.Left ?? Length.Auto;
				DisplayPanel.Style.Top = displayPosition.Value.Top ?? Length.Auto;
			}

			if ( displaySize.HasValue )
			{
				DisplayPanel.Style.Width = displaySize.Value.Width ?? Length.Auto;
				DisplayPanel.Style.Height = displaySize.Value.Height ?? Length.Auto;
			}
		}
	}

	protected override void OnMouseDown( MousePanelEvent e )
	{
		if ( !InfoPanel.IsValid() )
		{
			return;
		}

		HideInfo( InfoPanel.OnComplete );
		base.OnMouseDown( e );
	}

	public void HideInfo( System.Action? onComplete = null )
	{
		RemoveClass( "visible" );
		InfoPanel?.Delete();
		InfoPanel = null;
		onComplete?.Invoke();
	}

	public void SetInputLock( bool state )
	{
		if ( state )
		{
			AddClass( "input-lock" );
		}
		else
		{
			RemoveClass( "input-lock" );
		}
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( InfoPanel );
	}
}
