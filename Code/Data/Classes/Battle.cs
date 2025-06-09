namespace CardGame.Data;

public class Battle : IResource, IDeepCopyable<Battle>
{
	[InlineEditor]
	public Id Id { get; set; } = Id.Invalid;

	public string Name { get; set; } = string.Empty;

	public BattleType Type { get; set; } = BattleType.Normal;

	public SoundEvent? Bgm { get; set; }

	[InlineEditor, WideMode]
	public List<Id> PlayerUnits { get; set; } = [];

	[InlineEditor, WideMode]
	public List<BattleUnit> EnemyUnits { get; set; } = [];

	[InlineEditor, WideMode]
	public BattleRewards Rewards { get; set; } = new();

	public string Script { get; set; } = string.Empty;

	public Battle DeepCopy()
	{
		return new Battle()
		{
			Id = Id,
			Name = Name,
			Type = Type,
			Bgm = Bgm,
			EnemyUnits = [..EnemyUnits],
			Rewards = Rewards,
			Script = Script
		};
	}

	public override string ToString()
	{
		return $"Battle: {Name} - Id: {Id.LocalId}";
	}

	public enum BattleType
	{
		Normal,
		Elite,
		Boss
	}

	public class BattleUnit
	{
		public bool UseLevelScaling { get; set; } = true;

		public bool UseFloorLevel { get; set; } = true;

		[HideIf( nameof( UseFloorLevel ), true )]
		public int BaseLevel { get; set; } = 1;

		[InlineEditor]
		public Id Id { get; set; } = Id.Invalid;
	}
}

public class BattleRewards
{
	public List<Id> Cards { get; set; } = [];
	public List<Id> CardPacks { get; set; } = [];
	public List<Id> Relics { get; set; } = [];
	public int Money { get; set; } = 50;
}
