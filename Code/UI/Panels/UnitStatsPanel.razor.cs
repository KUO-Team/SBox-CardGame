using System;
using CardGame.Units;

namespace CardGame.UI;

public partial class UnitStatsPanel
{
	[Parameter]
	public BattleUnit? Unit { get; set; }
	
	public void Click()
	{
		Delete();
	}
	
	protected override int BuildHash()
	{
		return HashCode.Combine( Unit );
	}
}
