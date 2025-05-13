using System;

namespace CardGame.Data;

public class Relic : IResource, IDeepCopyable<Relic>
{
	[InlineEditor]
	public Id Id { get; set; } = Id.Invalid;

	[ImageAssetPath]
	public string Icon { get; set; } = string.Empty;

	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public List<string> Flavor { get; set; } = [];

	public TagSet Keywords { get; set; } = new();

	public RelicRarity Rarity { get; set; } = RelicRarity.Common;

	public RelicAvailabilities Availabilities { get; set; } = RelicAvailabilities.None;

	public string Script { get; set; } = string.Empty;

	public Relic DeepCopy()
	{
		return new Relic()
		{
			Id = Id,
			Icon = Icon,
			Name = Name,
			Description = Description,
			Flavor = [..Flavor],
			Rarity = Rarity,
			Script = Script
		};
	}

	public override string ToString()
	{
		return $"Relic: {Name} - Id: {Id.LocalId}";
	}

	public enum RelicRarity
	{
		Common,
		Uncommon,
		Rare,
		Epic
	}

	[Flags]
	public enum RelicAvailabilities
	{
		None = 0,
		[Description( "Found in shops" )]
		Shop = 1 << 0,
		[Description( "In starting deck" )]
		Starter = 1 << 1,
		[Description( "Given as a reward" )]
		Reward = 1 << 2,
		[Description( "Found in chests" )]
		Chest = 1 << 3,
		[Description( "Granted in special events" )]
		Event = 1 << 4,
		[Description( "Only available in dev builds/testing" )]
		DevOnly = 1 << 5,
	}
}
