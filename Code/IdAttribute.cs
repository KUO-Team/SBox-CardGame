using System;
using CardGame.Data;

namespace CardGame;

/// <summary>
/// Allows marking fields with an ID.
/// </summary>
[AttributeUsage( AttributeTargets.Field )]
public class IdAttribute( int id ) : Attribute
{
	public Id Id { get; } = id;
}
