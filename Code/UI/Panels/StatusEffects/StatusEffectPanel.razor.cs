using System;
using Sandbox.UI;
using CardGame.StatusEffects;

namespace CardGame.UI;

public partial class StatusEffectPanel
{
	[Parameter]
	public StatusEffect? Status { get; set; }
	
	private static Hud? Hud => UI.Hud.Instance;
	
	private StatusEffectInfoPanel? _infoPanel;
	
	public StatusEffectPanel()
	{
		AddClass( "status" );
	}
	
	public override void Delete( bool immediate = false )
	{
		Clear();
		base.Delete( immediate );
	}
	
	protected override void OnMouseOver( MousePanelEvent e )
	{
		_infoPanel = new StatusEffectInfoPanel
		{
			Status = Status
		};

		Hud?.ClearElements<StatusEffectInfoPanel>();
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
		Hud?.ClearElements<StatusEffectInfoPanel>();
	}
	
	protected override int BuildHash()
	{
		return HashCode.Combine( Status, Status?.Stack, Status?.Data.Name, Status?.Data.Description );
	}
}
