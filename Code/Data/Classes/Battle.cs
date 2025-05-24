namespace CardGame.Data;

public class Battle : IResource, IDeepCopyable<Battle>
{
	[InlineEditor]
	public Id Id { get; set; } = Id.Invalid;

	public string Name { get; set; } = string.Empty;

	public SoundEvent? Bgm { get; set; }

	public bool IsBoss { get; set; }

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
			Bgm = Bgm,
			IsBoss = IsBoss,
			EnemyUnits = [..EnemyUnits],
			Rewards = Rewards,
			Script = Script
		};
	}

	public override string ToString()
	{
		return $"Battle: {Name} - Id: {Id.LocalId}";
	}

	public class BattleUnit
	{
		public bool UseFloorLevelScaling { get; set; } = true;

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
