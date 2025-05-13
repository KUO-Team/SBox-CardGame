using System;
using CardGame.Units;
using Sandbox.UI;

namespace CardGame.UI;

public partial class UnitBattleInfoPanel
{
	[Property, TextArea] 
	public BattleUnit? Unit { get; set; }
	
	[Property] 
	public GameObject? AnchorObject { get; set; }
	
	private Panel? _main;
	
	protected override void OnUpdate()
	{
		if ( !_main.IsValid() || !AnchorObject.IsValid() )
		{
			return;
		}

		var position = Scene.Camera.PointToScreenNormal( AnchorObject.WorldPosition );
		_main.Style.Left = Length.Percent( position.x * 100f );
		_main.Style.Top = Length.Percent( position.y * 100f );

		base.OnUpdate();
	}
	
	protected override void OnDestroy()
	{
		StatusEffectPanel.Clear();
		base.OnDestroy();
	}
	
	protected override int BuildHash()
	{
		return HashCode.Combine(
			Unit,
			Unit?.Mana,
			Unit?.MaxMana,
			Unit?.HealthComponent?.Health,
			Unit?.HealthComponent?.MaxHealth,
			Unit?.HealthComponent?.HealthPercentage,
			Unit?.StatusEffects?.Count
		);
	}
}
