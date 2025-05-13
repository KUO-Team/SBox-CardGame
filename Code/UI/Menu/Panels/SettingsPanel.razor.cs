using System;
using Sandbox.Audio;
using Sandbox.UI;

namespace CardGame.UI;

public partial class SettingsPanel
{
	private static Mixer[] Mixers => Mixer.Master.GetChildren();
	
	public Panel? Tabs { get; set; }
	
	public Panel? OpenedTab { get; set; }

	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime )
		{
			return;
		}

		if ( !Tabs.IsValid() )
		{
			return;
		}
		
		HideAllTabs();
		OpenTab( Tabs.Children.FirstOrDefault()?.Id ?? string.Empty );
		
		base.OnAfterTreeRender( firstTime );
	}

	public void OpenTab( string id )
	{
		if ( !Tabs.IsValid() )
		{
			return;
		}
		
		OpenedTab = GetTabById( id );
		foreach ( var tab in Tabs.Children )
		{
			tab?.Hide();
		}
		OpenedTab?.Show();
	}

	public void HideAllTabs()
	{
		if ( !Tabs.IsValid() )
		{
			return;
		}
		
		foreach ( var tab in Tabs.Children )
		{
			tab?.Hide();
		}
		OpenedTab = null;
	}

	public Panel? GetTabById( string id )
	{
		return Tabs?.Children.FirstOrDefault( x => x.Id == id );
	}
	
	protected override int BuildHash()
	{
		return HashCode.Combine( 0 );
	}
}
