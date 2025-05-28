using System;
using Sandbox.UI;
using CardGame.Data;

namespace CardGame.UI;

public partial class RelicGainPanel
{
	public Relic? Relic { get; set; }

	private System.Action? _onConfirm;

	public void Show( Relic relic, System.Action? onConfirm = null )
	{
		Relic = relic;
		_onConfirm = onConfirm;
		this.Show();
	}

	public void Close()
	{
		this.Hide();
		_onConfirm?.Invoke();
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Relic );
	}
}
