namespace CardGame.Data;

public class StatusEffect : IResource
{
	[InlineEditor]
	public Id Id { get; set; } = Id.Invalid;

	[ImageAssetPath]
	public string Icon { get; set; } = string.Empty;

	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;
	
	public bool IsNegative { get; set; }
	
	public bool IsHiddenStack { get; set; }
	
	public Color Color { get; set; } = Color.FromBytes( 255, 215, 0 );
	
	public int? Maximum { get; set; }

	public string Script { get; set; } = string.Empty;
}
