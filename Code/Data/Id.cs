using System;
using System.Text.Json.Serialization;

namespace CardGame.Data;

public sealed class Id : IEquatable<Id>
{
	/// <summary>
	/// If this resource comes from a mod, this is usually the ident of the mod.
	/// </summary>
	public string Source { get; init; } = string.Empty;
	
	public int LocalId { get; init; }
 	
	[JsonIgnore, Hide]
	public bool IsFromMod => Source.Length > 0;
	
	[JsonIgnore, Hide]
	public bool IsValid => LocalId >= 0;

	public static Id Invalid => new() { LocalId = InvalidId };

	private const int InvalidId = -1;
	
	public bool Equals( Id? other )
	{
		return other is not null && other.Source == Source && other.LocalId == LocalId;
	}
	
	public override bool Equals( object? obj )
	{
		return Equals( obj as Id );
	}

	public override int GetHashCode()
	{
		return HashCode.Combine( Source, LocalId );
	}

	public override string ToString()
	{
		return IsFromMod ? $"{Source} : {LocalId}" : LocalId.ToString();
	}

	public static implicit operator int( Id id ) => id.LocalId;
	public static implicit operator Id( int id ) => new() { LocalId = id };
}
