using System;
using CardGame.Data;

namespace CardGame.UI;

public partial class PassiveAbilityPanel
{
	[Parameter]
	public PassiveAbility? Passive { get; set; }

	protected override int BuildHash()
	{
		return HashCode.Combine( Passive );
	}
}
