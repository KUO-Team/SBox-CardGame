using Sandbox.UI;

namespace CardGame.UI.Manual;

public partial class ManualPanel
{
	private Panel? _tabs;
	
	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime )
		{
			return;
		}

		if ( !_tabs.IsValid() )
		{
			return;
		}

		foreach ( var tab in _tabs.Children )
		{
			if ( !tab.IsValid() )
			{
				continue;
			}

			tab.Hide();
		}
		
		_tabs.Children.FirstOrDefault()?.Show();
		base.OnAfterTreeRender( firstTime );
	}

	public void Close()
	{
		Delete();
	}
	
	public Panel? GetTabById( string id )
	{
		if ( !_tabs.IsValid() )
		{
			return null;
		}

		return _tabs.Children.FirstOrDefault( x => x.Id == id );
	}
}
