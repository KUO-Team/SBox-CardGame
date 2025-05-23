using System;
using Sandbox.UI;
using CardGame.Data;

namespace CardGame.UI;

public partial class UnitInfoPanel
{
	[Parameter]
	public Unit? Unit { get; set; }
		
	public Panel? SelectedTab { get; private set; }

	private Panel? _tabContainer;

	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime )
		{
			return;
		}

		if ( !_tabContainer.IsValid() )
		{
			return;
		}
		
		HideAllTabs();

		var firstTab = _tabContainer.Children.FirstOrDefault();
		if ( firstTab.IsValid() )
		{
			ChangeTabs( firstTab.Id );
		}
		
		base.OnAfterTreeRender( firstTime );
	}

	public void Close()
	{
		this.Hide();
	}

	public void ChangeTabs( string id )
	{
		if ( !_tabContainer.IsValid() )
		{
			return;
		}

		var selectedTab = GetTabById( id );
		HideAllTabs();

		if ( selectedTab.IsValid() )
		{
			selectedTab.Show();
			SelectedTab = selectedTab;
		}
	}
	
	private void HideAllTabs()
	{
		if ( !_tabContainer.IsValid() )
		{
			return;
		}

		foreach ( var tab in _tabContainer.Children )
		{
			tab?.Hide();
		}
	}

	public Panel? GetTabById( string id )
	{
		return _tabContainer?.Children.FirstOrDefault( tab => tab.Id == id );
	}
	
	protected override int BuildHash()
	{
		return HashCode.Combine( Unit, SelectedTab );
	}
}
