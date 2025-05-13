using Sandbox.UI;
using CardGame.Units;

namespace CardGame.UI;

public partial class UnitPanel
{
	[Property]
	public BattleUnit? Unit { get; set; }
	
	protected override void OnMouseDown( MousePanelEvent e )
	{
		var hud = Hud.Instance;
		if ( hud.IsValid() )
		{
			var panel = new UnitStatsPanel
			{
				Unit = Unit
			};
			hud.AddElement( panel );
		}

		base.OnMouseDown( e );
	}
}
