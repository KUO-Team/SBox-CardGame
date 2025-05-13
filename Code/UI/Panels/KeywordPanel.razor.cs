using System;
using CardGame.Data;

namespace CardGame.UI;

public partial class KeywordPanel
{
	public Keyword? Keyword { get; set; }

	public KeywordPanel()
	{
		
	}

	public KeywordPanel( Keyword keyword )
	{
		Keyword = keyword;
	}
	
	protected override int BuildHash()
	{
		return HashCode.Combine( Keyword );
	}
}
