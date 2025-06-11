using System;
using Sandbox.UI;
using CardGame.Units;

namespace CardGame.UI;

public partial class CardSlotsPanel
{
	[Property, RequireComponent]
	public CardSlotList? Slots { get; set; }
	
	[Property]
	public GameObject? AnchorObject { get; set; }
	
	private Panel? _main;
	
	public void Init()
	{
		if ( !_main.IsValid() )
		{
			return;
		}

		foreach ( var slot in _main.ChildrenOfType<CardSlotPanel>() )
		{
			slot.Init();
		}
	}
	
	protected override void OnTreeFirstBuilt()
	{
		Init();
		base.OnTreeFirstBuilt();
	}
	
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
	
	protected override int BuildHash()
	{
		return HashCode.Combine( Slots, Slots?.Count, Slots?.Owner, Slots?.HashCombine( x => x.Speed ), Slots?.Owner?.HealthComponent?.IsDead );
	}
}
