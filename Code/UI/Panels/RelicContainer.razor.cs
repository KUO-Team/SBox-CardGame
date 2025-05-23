using System;

namespace CardGame.UI;

public partial class RelicContainer
{
	private static RelicManager? RelicManager => RelicManager.Instance;
	
	private static List<Relics.Relic> Relics => RelicManager?.Relics ?? [];

	protected override int BuildHash()
	{
		return HashCode.Combine( RelicManager, Relics.Count );
	}
}
