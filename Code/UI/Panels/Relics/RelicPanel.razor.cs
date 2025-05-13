using System;
using Sandbox.UI;

namespace CardGame.UI;

public partial class RelicPanel
{
	[Parameter]
	public Data.Relic? Relic { get; set; }
	
	private static Hud? Hud => UI.Hud.Instance;
	
	private RelicInfoPanel? _infoPanel;

	public override void Delete( bool immediate = false )
	{
		Clear();
		base.Delete( immediate );
	}

	protected override void OnMouseOver( MousePanelEvent e )
	{
		_infoPanel = new RelicInfoPanel
		{
			Relic = Relic
		};

		Hud?.ClearElements<RelicInfoPanel>();
		Hud?.AddElement( _infoPanel );
		base.OnMouseOver( e );
	}
	
	protected override void OnMouseOut( MousePanelEvent e )
	{
		Clear();
		base.OnMouseOut( e );
	}
	
	public static void Clear()
	{
		Hud?.ClearElements<RelicInfoPanel>();
	}
	
	protected override int BuildHash()
	{
		return HashCode.Combine( Relic, Relic?.Name, Relic?.Description );
	}
}
