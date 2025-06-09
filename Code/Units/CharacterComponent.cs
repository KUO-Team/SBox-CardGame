using SpriteTools;

namespace CardGame;

public abstract class CharacterComponent : Component
{
	[Property, RequireComponent, Category( "Components" )]
	public SpriteComponent? SpriteComponent { get; set; }
}
