using System;

namespace CardGame;

public interface ILeveling
{
	int Level { get; }
	float Experience { get; }
	float ExperienceToNextLevel { get; }

	void GainExperience( float amount );
	event System.Action<int> OnLevelUp;
}
