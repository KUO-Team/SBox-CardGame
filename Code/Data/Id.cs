using System;
using System.Text.Json.Serialization;

namespace CardGame.Data;

public sealed record Id( string Source, int LocalId ) : IComparable<Id>
{
	[JsonIgnore, Hide]
	public bool IsFromMod => Source.Length > 0;

	[JsonIgnore, Hide]
	public bool IsValid => LocalId >= 0;

	public static Id Invalid => new( string.Empty, -1 );

	public int CompareTo( Id? other )
	{
		if ( other is null )
		{
			return 1;
		}

		var sourceComparison = string.Compare( Source, other.Source, StringComparison.Ordinal );
		return sourceComparison != 0 ? sourceComparison : LocalId.CompareTo( other.LocalId );
	}
	
	public override string ToString()
	{
		return IsFromMod ? $"{Source} : {LocalId}" : LocalId.ToString();
	}

	public static implicit operator int( Id id ) => id.LocalId;
	public static implicit operator Id( int id ) => new( string.Empty, id );
}
