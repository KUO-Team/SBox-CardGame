using System;
using System.Text.Json.Serialization;

namespace CardGame;

[Serializable]
public sealed class RangedInt : IEquatable<RangedInt>
{
	public int Min { get; set; }
	
	public int Max { get; set; }
	
	/// <summary>
	/// Get a random value in the range.
	/// </summary>
	[Hide, JsonIgnore]
	public int Value
	{
		get
		{
			return Game.Random.Next( Min, Max + 1 );
		}
	}
	
	[Hide, JsonIgnore]
	public bool IsRandom
	{
		get
		{
			return Min != Max;
		}
	}

	public RangedInt( int min, int max )
	{
		if ( min > max )
		{
			throw new ArgumentException( "Min cannot be greater than Max." );
		}

		Min = min;
		Max = max;
	}
	
	public int GetRandom()
	{
		return Value;
	}

	public bool Contains( int value )
	{
		return value >= Min && value <= Max;
	}

	public int Clamp( int value )
	{
		if ( value < Min )
		{
			return Min;
		}
		
		if ( value > Max )
		{
			return Max;
		}
		
		return value;
	}

	public override string ToString()
	{
		return $"[{Min}, {Max}]";
	}

	// Implicit conversion from int (min = max)
	public static implicit operator RangedInt( int value )
	{
		return new RangedInt( value, value );
	}

	// Implicit conversion from tuple (min, max)
	public static implicit operator RangedInt( (int min, int max) range )
	{
		return new RangedInt( range.min, range.max );
	}

	// Explicit conversion to int (random value)
	public static explicit operator int( RangedInt r )
	{
		return r.Value;
	}

	public static bool operator ==( RangedInt a, RangedInt b )
	{
		return a.Min == b.Min && a.Max == b.Max;
	}

	public static bool operator !=( RangedInt a, RangedInt b )
	{
		return !(a == b);
	}

	public override bool Equals( object? obj )
	{
		return obj is RangedInt other && Equals( other );
	}

	public bool Equals( RangedInt? other )
	{
		return other is not null && this == other;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine( Min, Max );
	}
}
