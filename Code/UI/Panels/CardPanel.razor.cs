using System;

namespace CardGame.UI;

public partial class CardPanel
{
	[Parameter]
	public Card? Card { get; set; }

	public CardPanel()
	{
		AddClass( "card" );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine(
			Card,
			Card?.EffectiveCost
		);
	}
}
