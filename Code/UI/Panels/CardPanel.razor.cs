using System;
using Sandbox.UI;

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
			Card?.Name,
			Card?.Cost,
			Card?.EffectiveCost,
			Card?.Rarity,
			Card?.Actions.Count,
			string.Join( ",", Card?.Actions.Select( a => a.Type + (int)a.EffectivePower + (a.Effect?.Description ?? "") ) ?? Array.Empty<string>() )
		);
	}
}
