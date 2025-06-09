using System;

namespace CardGame.Units;

public class LevelComponent : Component, ILeveling, IOwnable
{
	[Property, RequireComponent]
	public BattleUnitComponent? Owner { get; set; }

	[Property, Category( "State" )]
	public int Level
	{
		get => _level;
		set => _level = Math.Clamp( value, 1, MaxLevel );
	}
	private int _level = 1;

	[Property, Category( "State" )]
	public float Experience
	{
		get => _experience;
		set => _experience = Math.Max( value, 0 );
	}
	private float _experience = 0;

	[Property, Category( "State" )]
	public float ExperienceToNextLevel
	{
		get => _experienceToNextLevel;
		set => _experienceToNextLevel = Math.Max( value, 1 );
	}
	private float _experienceToNextLevel = DefaultExperienceToNextLevel;

	public event Action<int>? OnLevelUp;
	public event Action<float>? OnExperienceGained;
	
	public const int MaxLevel = 100;
	public const int DefaultExperienceToNextLevel = 100;
	public const float ExperienceScaleFactor = 1.2f;
	
	public void GainExperience( float amount )
	{
		if ( !Owner.IsValid() )
		{
			Log.Warning( $"GainExperience called with invalid Owner on {this}" );
			return;
		}

		if ( amount <= 0 )
		{
			Log.Warning( $"Tried to gain non-positive experience ({amount}) on {this}" );
			return;
		}

		if ( Level >= MaxLevel )
		{
			return; // Max level reached, XP gain ignored
		}

		Experience += amount;
		OnExperienceGained?.Invoke( amount );

		while ( Experience >= ExperienceToNextLevel && Level < MaxLevel )
		{
			Experience -= ExperienceToNextLevel;
			PerformLevelUp();
		}
	}

	private void PerformLevelUp()
	{
		Level++;

		OnLevelUp?.Invoke( Level );

		if ( Level < MaxLevel )
		{
			ExperienceToNextLevel = (int)Math.Ceiling( ExperienceToNextLevel * ExperienceScaleFactor );
		}
		else
		{
			// Clamp XP to zero at max level
			Experience = 0;
		}
	}

	protected override void OnStart()
	{
		base.OnStart();

		if ( Owner is not null )
		{
			Owner.LevelComponent = this;
		}

		OnLevelUp += HandleLevelUp;
	}

	protected override void OnDestroy()
	{
		if ( Owner is not null )
		{
			Owner.LevelComponent = null;
		}

		OnLevelUp -= HandleLevelUp;
		base.OnDestroy();
	}

	private void HandleLevelUp( int newLevel )
	{
		Log.Info( $"Unit {Owner?.GameObject.Name} leveled up to {newLevel}!" );
	}

	public override string ToString()
	{
		return $"{nameof( LevelComponent )}(Owner: {Owner?.GameObject.Name}, Level: {Level}, XP: {Experience}/{ExperienceToNextLevel})";
	}
}
