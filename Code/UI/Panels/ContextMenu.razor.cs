using System;
using Sandbox.UI;

namespace CardGame.UI;

public partial class ContextMenu
{
	private static ContextMenu? ActivePanel { get; set; }
	
	private readonly List<MenuItem> _menuItems = [];
	
	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime )
		{
			return;
		}

		ActivePanel?.Delete();
		ActivePanel = this;

		var (screenWidth, screenHeight) = (Screen.Width, Screen.Height);
		var (width, height) = (Box.Rect.Width, Box.Rect.Height);
		var (left, top) = (Mouse.Position.x, Mouse.Position.y);

		var adjustedX = Math.Clamp( left, 0, screenWidth - width );
		var adjustedY = Math.Clamp( top, 0, screenHeight - height );

		Style.Left = Length.Percent( (adjustedX / screenWidth) * 100 );
		Style.Top = Length.Percent( (adjustedY / screenHeight) * 100 );

		base.OnAfterTreeRender( firstTime );
	}
	
	public void AddItem( string name, System.Action? onClick = null, List<MenuItem>? subMenu = null )
	{
		var item = new MenuItem
		{
			Name = name, OnClick = onClick, SubMenu = subMenu
		};

		_menuItems.Add( item );
	}
	
	private static void HandleItemClick( MenuItem item )
	{
		if ( item.SubMenu is not null )
		{
			item.IsSubMenuOpen = !item.IsSubMenuOpen;
		}
		else
		{
			item.OnClick?.Invoke();
		}
	}

	public class MenuItem : Panel
	{
		public string Name { get; set; } = string.Empty;
		public new System.Action? OnClick { get; set; }
		public List<MenuItem>? SubMenu { get; set; }
		public bool IsSubMenuOpen { get; set; }
	}
}
