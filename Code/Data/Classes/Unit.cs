namespace CardGame.Data;

public class Unit : IResource, IDeepCopyable<Unit>
{
	[InlineEditor]
	public Id Id { get; set; } = Id.Invalid;

	public string Name { get; set; } = "";

	public string Description { get; set; } = "";

	[InlineEditor]
	public SpriteInfo? Sprite { get; set; }
	
	public GameObject? Prefab { get; set; }

	public int Level { get; set; } = 1;

	public int Hp { get; set; }
	
	public int Ep { get; set; }

	public int Mp { get; set; } = 0;
	
	[InlineEditor]
	public RangedInt Speed { get; set; } = (1, 6);

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
			Hp = Hp,
			Ep = Ep,
			Mp = Mp,
			Speed = Speed,
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
