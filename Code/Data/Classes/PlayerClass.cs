namespace CardGame.Data;

public class PlayerClass : IResource
{
	[InlineEditor]
	public Id Id { get; set; } = Id.Invalid;
	
	public string Name { get; set; } = string.Empty;
	
	public string Description { get; set; } = string.Empty;
	
	[InlineEditor]
	public Id Unit { get; set; } = Id.Invalid;
}
