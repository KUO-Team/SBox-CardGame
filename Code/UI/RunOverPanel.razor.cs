using System;

namespace CardGame.UI;

public partial class RunOverPanel
{
	public static List<string> VictoryQuotes { get; } =
	[
		"You reached the heart. It beat once. Then waited.",
		"The descent ends in silence. But silence is never empty.",
		"You brought the light. It did not belong here.",
		"Whatever was sealed below now carries your name.",
		"You won. At a cost only the abyss could count.",
		"Few reach this far. Fewer are allowed to leave."
	];

	public static Dictionary<int, List<string>> FloorQuotes { get; } = new()
	{
		{
			0, [
				"The silence is complete. But something still listens.",
				"The last gate yields. What was kept below, no longer is.",
				"Every step was a key. Now the door stands open.",
				"This is not an ending. It's just the deepest beginning."
			]
		},
		{
			1, [
				"The threshold hums, almost alive.",
				"You paused here. It remembers that.",
				"Almost beneath it all. Almost too late.",
				"This floor was meant to hold a warning. But time erased it."
			]
		},
		{
			2, [
				"This was once a sanctuary. Now, it only echoes.",
				"Here, the first ones broke. Their marks remain.",
				"A forgotten war was fought on this level. And lost.",
				"Old symbols on the walls flicker, just briefly, as you pass."
			]
		},
		{
			3, [
				"The descent begins where hope still lingers.",
				"Sunlight touches this place. Barely.",
				"Every path down was carved in desperation.",
				"The first step is always chosen. The rest are taken."
			]
		}
	};
	
	public static List<string> GeneralQuotes { get; } = new()
	{
		"Each floor remembers the weight of those who passed.",
		"The deeper you go, the less of yourself returns.",
		"You didn’t descend alone. Something followed.",
		"The walls whisper in languages no one alive should know.",
		"Not madness. Just clarity twisted too far.",
		"It never wanted to be found.",
		"The past is buried here—yours, or someone else's.",
		"The layers shift when you aren’t looking.",
		"You thought you were searching for something. It was searching too.",
		"You left a part of yourself behind. The wrong part came back.",
		"Whatever sleeps down here dreams of you now.",
		"All roads lead to the center. None return from it."
	};
	
	private string _quote = string.Empty;

	private System.Action? _onReturn;
	
	public void EndRun( System.Action? onReturn = null )
	{
		_quote = GetRandomQuote();
		_onReturn = onReturn;
	}
	
	public static string GetRandomQuote()
	{
		if ( GameManager.Instance is not { } gameManager )
		{
			return string.Empty;
		}

		var floor = gameManager.Floor;

		bool useFloorQuote = Game.Random.Float( 0f, 1f ) < 0.2f;

		if ( useFloorQuote && FloorQuotes.TryGetValue( floor, out var quotesForFloor ) && quotesForFloor.Count > 0 )
		{
			var index = Game.Random.Int( 0, quotesForFloor.Count - 1 );
			return quotesForFloor[index];
		}

		if ( GeneralQuotes.Count > 0 )
		{
			var index = Game.Random.Int( 0, GeneralQuotes.Count - 1 );
			return GeneralQuotes[index];
		}

		return "Even the abyss is speechless.";
	}
	
	public void ReturnToMenu()
	{
		if ( SceneManager.Instance is not {} sceneManager )
		{
			return;
		}

		var menuScene = sceneManager.MenuScene;
		if ( menuScene is not null )
		{
			GameObject.Enabled = false;
			_onReturn?.Invoke();
			Scene.Load( menuScene );
		}
	}
	
	protected override int BuildHash()
	{
		return HashCode.Combine( _quote );
	}
}
