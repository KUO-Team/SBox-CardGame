using Sandbox.UI;

namespace CardGame.UI;

[Icon( "desktop_windows" )]
[EditorHandle( "materials/gizmo/ui.png" )]
[Description( "Represents a full screen empty canvas." )]
public class Hud : PanelComponent
{
	public static Hud? Instance { get; private set; }

	protected override void OnStart()
	{
		Instance = this;
		GameObject.Flags = GameObjectFlags.DontDestroyOnLoad;

		foreach ( var element in TypeLibrary.GetAttributes<HudAttribute>() )
		{
			var instance = TypeLibrary.Create<Panel>( element.TargetType );
			if ( instance is null )
			{
				continue;
			}

			AddElement( instance, element.Hidden );
		}

		base.OnStart();
	}

	public void AddElement( Panel element, bool hiddenByDefault = false )
	{
		Panel.AddChild( element );

		if ( hiddenByDefault )
		{
			element.Hide();
		}
	}

	public T AddElement<T>() where T : Panel, new()
	{
		var element = new T();
		Panel.AddChild( element );

		return element;
	}

	public void ClearElements( bool immediate = false )
	{
		Panel.DeleteChildren( immediate );
	}

	public void ClearElements<T>( bool immediate = false ) where T : Panel
	{
		if ( !Panel.IsValid() )
		{
			return;
		}

		foreach ( var element in Panel.ChildrenOfType<T>().ToList() )
		{
			element?.Delete( immediate );
		}
	}

	public bool HasElement<T>() where T : Panel
	{
		return Panel.ChildrenOfType<T>().Any();
	}

	public List<Panel> GetElements()
	{
		return Panel.Children.ToList();
	}

	public List<T> GetElements<T>() where T : Panel
	{
		return Panel.ChildrenOfType<T>().ToList();
	}

	public T? GetFirstElement<T>() where T : Panel
	{
		return Panel.ChildrenOfType<T>().FirstOrDefault();
	}

	public static T? GetElement<T>() where T : Panel
	{
		return !Hud.Instance.IsValid() ? null : Hud.Instance.GetFirstElement<T>();
	}

	private void StyleHack()
	{
		var root = Panel.FindRootPanel();
		foreach ( var stylesheet in root.AllStyleSheets.ToList() )
		{
			root.StyleSheet.Remove( stylesheet );
		}

		root.StyleSheet.Load( "/UI/Hud/Hud.cs.scss" );
	}
}
