using System;
using Sandbox.UI;
using CardGame.StatusEffects;

namespace CardGame.UI;

public partial class StatusEffectInfoPanel
{
	[Parameter]
	public StatusEffect? Status { get; set; }
	
	public override void Tick()
	{
		var mousePosition = Mouse.Position;
		var bounds = Box.Rect;

		var maxX = 100 - (bounds.Width / Screen.Width * 100);
		var maxY = 100 - (bounds.Height / Screen.Height * 100);

		var relativeX = Math.Clamp( mousePosition.x / Screen.Width * 100, 0, maxX );
		var relativeY = Math.Clamp( mousePosition.y / Screen.Height * 100, 0, maxY );

		Style.Left = Length.Percent( relativeX );
		Style.Top = Length.Percent( relativeY );

		base.Tick();
	}
}
