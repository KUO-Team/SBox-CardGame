namespace CardGame.Data;

public class PassiveAbility : IResource, IDeepCopyable<PassiveAbility>
{
	[InlineEditor]
	public Id Id { get; set; } = Id.Invalid;
	
	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;

	public string Script { get; set; } = string.Empty;

	public PassiveAbility DeepCopy()
	{
		return new PassiveAbility
		{
			Id = Id,
			Name = Name, 
			Description = Description,
			Script = Script
		};
	}
}
