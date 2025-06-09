using System;
using System.Threading.Tasks;
using CardGame.StatusEffects;

namespace CardGame.Units;

public class HealthComponent : Component, IOwnable
{
	[Property, RequireComponent]
	public BattleUnitComponent? Owner { get; set; }

	[Property, Category( "State" )]
	public int Health
	{
		get => _health;
		set => _health = Math.Clamp( value, 0, MaxHealth );
	}
	private int _health = 100;

	[Property, Category( "State" )]
	public int MaxHealth
	{
		get => _maxHealth;
		set => _maxHealth = Math.Max( value, 1 );
	}
	private int _maxHealth = 100;

	[Property, Category( "State" )]
	public bool IsDead => Health <= 0;

	[Property, Category( "State" )]
	public float HealthPercentage => (float)Health / MaxHealth * 100;

	[Property, Category( "Prefabs" )]
	public GameObject? DamageNumbersPrefab { get; set; }

	public event Action<int>? OnTakeDamage;
	public event Action<int>? OnHeal;
	public event Func<Task>? OnDied;

	public void TakeDamage( int damage, BattleUnitComponent? attacker = null, Card? card = null )
	{
		if ( !Owner.IsValid() )
		{
			return;
		}

		if ( IsDead || damage <= 0 )
		{
			return;
		}

		if ( Owner.StatusEffects.IsValid() )
		{
			foreach ( var status in Owner.StatusEffects )
			{
				damage += status.DamageModifier( damage, card );
				damage = Math.Max( damage, 0 );
			}
		}

		Health = Math.Max( Health - damage, 0 );
		OnTakeDamage?.Invoke( damage );
		ProcessDamageEffects( damage, attacker );
		DamageNumbers( damage );

		if ( Health == 0 )
		{
			_ = Die();
		}
	}

	public void TakeFixedDamage( int damage, BattleUnitComponent? attacker = null, StatusEffect? statusEffect = null )
	{
		if ( !Owner.IsValid() )
		{
			return;
		}

		if ( IsDead || damage <= 0 )
		{
			return;
		}


		Health = Math.Max( Health - damage, 0 );
		OnTakeDamage?.Invoke( damage );
		ProcessDamageEffects( damage, attacker );
		DamageNumbers( damage );

		if ( Health == 0 )
		{
			_ = Die();
		}
	}

	public void Heal( int amount )
	{
		Health = Math.Min( Health + amount, MaxHealth );
		OnHeal?.Invoke( amount );
		DamageNumbers( amount, true );
	}

	public async Task Die()
	{
		if ( OnDied is not null )
		{
			foreach ( var @delegate in OnDied.GetInvocationList() )
			{
				try
				{
					var handler = (Func<Task>)@delegate;
					await handler();
				}
				catch ( Exception ex )
				{
					Log.Warning( $"Die called an exception: {ex}" );
				}
			}
		}
	}

	private void DamageNumbers( int amount, bool healing = false )
	{
		if ( !DamageNumbersPrefab.IsValid() )
		{
			return;
		}

		var damageNumbers = DamageNumbersPrefab.Clone();
		var component = damageNumbers.GetComponent<DamageNumbers>();

		if ( !component.IsValid() || !component.Text.IsValid() )
		{
			return;
		}

		component.Init( GameObject );

		if ( !healing )
		{
			component.Text.Text = $"-{amount}HP";
			component.Text.Color = Color.Red;
		}
		else
		{
			component.Text.Text = $"+{amount}HP";
			component.Text.Color = Color.Green;
		}
	}

	private void ProcessDamageEffects( int damage, BattleUnitComponent? attacker = null )
	{
		if ( !Owner.IsValid() )
		{
			return;
		}

		var ownerPassives = Owner.Passives;
		if ( ownerPassives.IsValid() )
		{
			foreach ( var passive in ownerPassives )
			{
				if ( !attacker.IsValid() )
				{
					continue;
				}

				passive.OnTakeDamage( damage, attacker );
			}
		}

		if ( attacker.IsValid() )
		{
			var attackerPassives = attacker.Passives;
			if ( attackerPassives.IsValid() )
			{
				foreach ( var passive in attackerPassives )
				{
					passive.OnDealDamage( damage, Owner );
				}
			}
		}

		if ( RelicManager.Instance.IsValid() )
		{
			foreach ( var relic in RelicManager.Instance.Relics )
			{
				relic.OnTakeDamage( damage, Owner, attacker );
				relic.OnDealDamage( damage, Owner, attacker );
			}
		}
	}
}
