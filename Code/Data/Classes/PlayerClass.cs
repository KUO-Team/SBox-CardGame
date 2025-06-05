namespace CardGame.Data;

public class PlayerClass : IResource, IDeepCopyable<PlayerClass>
{
	[InlineEditor]
	public Id Id { get; set; } = Id.Invalid;
	
	public string Name { get; set; } = string.Empty;
	
	public string Description { get; set; } = string.Empty;
	
	[InlineEditor]
	public Id Unit { get; set; } = Id.Invalid;

	[InlineEditor]
	public List<Id> Relics { get; set; } = [];
	
	public bool IsUnlocked { get; set; } = true;
	
	public PlayerClass DeepCopy()
	{
		return new PlayerClass
		{
			Id = Id,
			Name = Name,
			Description = Description,
			Unit = Unit,
			Relics = [..Relics],
			IsUnlocked = IsUnlocked
		};
	}
}
