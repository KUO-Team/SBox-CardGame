using System;
using Sandbox.UI;
using CardGame.StatusEffects;

namespace CardGame.UI;

public partial class StatusEffectDisplay
{
	[Parameter]
	public StatusEffect? Status { get; set; }
	
	private static Hud? Hud => Hud.Instance;
	
	private static StatusEffectInfoPanel? _infoPanel;
	
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
	}
	
	protected override void OnMouseOut( MousePanelEvent e )
	{
		Clear();
	}
	
	private static void Clear()
	{
		Hud?.ClearElements<StatusEffectInfoPanel>();
	}
	
	protected override int BuildHash()
	{
		return HashCode.Combine( Status );
	}
}
