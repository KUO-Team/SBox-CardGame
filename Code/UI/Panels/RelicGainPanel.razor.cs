using System;
using Sandbox.UI;
using CardGame.Data;

namespace CardGame.UI;

public partial class RelicGainPanel
{
	public Relic? Relic { get; set; }

	public void Show( Relic relic )
	{
		Relic = relic;
		this.Show();
	}

	public void Close()
	{
		this.Hide();
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Relic );
	}
}
