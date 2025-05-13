using System;

namespace CardGame.Data;

public class KeywordList
{
	public List<Keyword> List { get; set; } = [];
}

public static class KeywordLookup
{
	private const string FilePath = "/data/Keywords.json";
	
	/// <summary>
	/// Finds a keyword by name
	/// </summary>
	/// <param name="keywordName">The name of the keyword to find</param>
	/// <returns>The matching keyword or null if not found</returns>
	public static Keyword? FindKeyword( string keywordName )
	{
		try
		{
			var json = FileSystem.Mounted.ReadJson( FilePath, new KeywordList() );
			return json?.List.FirstOrDefault( k => string.Equals( k.Name, keywordName, StringComparison.CurrentCultureIgnoreCase ) );
		}
		catch ( Exception ex )
		{
			Log.Warning( $"Error looking up keyword: {ex.Message}" );
			return null;
		}
	}
}

public class Keyword
{
	public string Name { get; set; } = string.Empty;

	public string Description { get; set; } = string.Empty;
	
	public Dictionary<string, string>? Markers { get; set; }
}
