using SpriteTools;

namespace CardGame;

public class BaseCharacter : Component
{
	[Property, RequireComponent, Category( "Components" )]
	public SpriteComponent? SpriteComponent { get; set; }
}
