using Sandbox.Services;

namespace CardGame.Platform;

public enum Achievement
{
	[Title( "floor-3" )]
	Floor3,
	[Title( "floor-2" )]
	Floor2,
	[Title( "floor-1" )]
	Floor1,
	[Title( "floor-0" )]
	Floor0,
	[Title( "die-tutorial" )]
	DieInTutorial,
	[Title( "all-cards" )]
	AllCards,
	[Title( "all-relics" )]
	AllRelics,
	[Title( "collector" )]
	Collector
}

public static class Platform
{
	public static bool AllowAchievements => !Game.IsEditor && !Game.CheatsEnabled;

	public static bool CheatedRun { get; set; }
}

public static class Stats
{
	public static void Increment( string name, double amount, string? context = null, object? data = null )
	{
		if ( !Platform.AllowAchievements )
		{
			return;
		}

		if ( Platform.CheatedRun )
		{
			return;
		}

		Sandbox.Services.Stats.Increment( name, amount, context, data );
		Log.Info( $"Incremented stat {name}" );
	}
}

public static class AchievementExtensions
{
	public static void Unlock( this Achievement achievement )
	{
		if ( !Platform.AllowAchievements )
		{
			return;
		}

		if ( Platform.CheatedRun )
		{
			return;
		}

		var title = achievement.GetAttributeOfType<TitleAttribute>();
		var ident = title.Value ?? achievement.ToString();
		Achievements.Unlock( ident );
		Log.Info( $"Unlocked achievement {ident}" );
	}
}
