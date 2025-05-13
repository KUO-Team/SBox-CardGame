using System;
using CardGame.Data;

namespace CardGame.UI;

public partial class LoadRunPanel
{
	private static PlayerData Data => PlayerData.Data;
	
	protected override int BuildHash()
	{
		return HashCode.Combine( Data, Data.Runs.Count );
	}
}
