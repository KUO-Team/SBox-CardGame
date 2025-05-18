namespace CardGame.Data;

public class Unit : IResource, IDeepCopyable<Unit>
{
	[InlineEditor]
	public Id Id { get; set; } = Id.Invalid;

	public string Name { get; set; } = "";

	public string Description { get; set; } = "";

	[InlineEditor]
	public SpriteInfo? Sprite { get; set; }
	
	public PrefabFile? Prefab { get; set; }

	public int Level { get; set; } = 1;

	public bool LevelScaling { get; set; } = true;

	public int Hp { get; set; }
	
	public int Ep { get; set; }

	public bool UseMp { get; set; }

	[ShowIf( nameof( UseMp ), true )]
	public int Mp { get; set; } = 0;
	
	public int MinSpeed { get; set; } = 1;

	public int MaxSpeed { get; set; } = 6;

	/// <summary>
	/// How many card slots does this unit start with?
	/// </summary>
	public int Slots { get; set; } = 1;

	[InlineEditor, WideMode]
	public List<Id> Deck { get; set; } = [];

	[InlineEditor, WideMode]
	public List<Id> Passives { get; set; } = [];

	public Unit DeepCopy()
	{
		return new Unit
		{
			Id = Id,
			Name = Name,
			Description = Description,
			Sprite = Sprite,
			Prefab = Prefab,
			Level = Level,
			LevelScaling = LevelScaling,
			Hp = Hp,
			Ep = Ep,
			UseMp = UseMp,
			Mp = Mp,
			MinSpeed = MinSpeed,
			MaxSpeed = MaxSpeed,
			Slots = Slots,
			Deck = [..Deck],
			Passives = [..Passives],
		};
	}
	
	public override string ToString()
	{
		return $"Unit: {Name} - Id: {Id.LocalId}";
	}

	public class SpriteInfo
	{
		public SpriteTools.SpriteResource? Resource { get; set; }
		
		[ImageAssetPath]
		public string Portrait { get; set; } = string.Empty;
	}
}
