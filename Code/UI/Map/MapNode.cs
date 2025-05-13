using System;
using Sandbox.UI;
using CardGame.Data;

namespace CardGame.UI.Map;

public class MapNode : Panel
{
	[Parameter]
	public int Index { get; set; }

	[Parameter]
	public bool Completed { get; set; }
	
	[Parameter]
	public bool Enabled { get; set; } = true;
	
	[Parameter]
	public MapNodeType Type { get; set; } = MapNodeType.Battle;

	[Parameter]
	public Id? Battle { get; set; }

	private Label? _label;

	public MapNode()
	{
		AddClass( "node" );
	}

	protected override void OnParametersSet()
	{
		Init();
		base.OnParametersSet();
	}

	private void Init()
	{
		DeleteChildren( true );

		if ( Type == MapNodeType.Battle )
		{
			Battle = Game.Random.Next( 0, 3 );
		}

		_label = new Label
		{
			Text = Type switch
			{
				MapNodeType.Battle => "B",
				MapNodeType.Shop => "S",
				MapNodeType.Event => "?",
				MapNodeType.Boss => "!",
				_ => "?"
			}
		};

		AddChild( _label );
	}

	protected override void OnClick( MousePanelEvent e )
	{
		if ( !Enabled )
		{
			return;
		}
		
		base.OnClick( e );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Type, Battle );
	}

	public enum MapNodeType
	{
		Start,
		Battle,
		Shop,
		Boss,
		Event
	}
}
