using System;
using CardGame.Data;

namespace CardGame.UI;

public partial class UnitInfoPanel
{
	[Parameter]
	public Unit? Unit { get; set; }
	
	protected override int BuildHash()
	{
		return HashCode.Combine( Unit );
	}
}
