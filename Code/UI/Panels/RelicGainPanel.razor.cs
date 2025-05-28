using System;
using Sandbox.UI;
using CardGame.Data;

namespace CardGame.UI;

public partial class RelicGainPanel
{
	public Relic? Relic { get; set; }
	
	public System.Action? OnConfirm { get; private set; }

	public void Show( Relic relic, System.Action? onConfirm = null )
	{
		Relic = relic;
		OnConfirm = onConfirm;
		this.Show();
	}

	public void Close()
	{
		OnConfirm?.Invoke();
		this.Hide();
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Relic );
	}
}
