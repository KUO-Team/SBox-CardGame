using System;
using System.Threading.Tasks;
using CardGame.StatusEffects;

namespace CardGame.Units;

public class HealthComponent : Component
{
	[Property, Category( "State" )]
	public int Health { get; set; } = 100;

	[Property, Category( "State" )]
	public int MaxHealth { get; set; } = 100;

	[Property, Category( "State" )]
	public bool IsDead { get; set; }

	[Property, Category( "State" )]
	public float HealthPercentage => (float)Health / MaxHealth * 100;

	[Property, Category( "Prefabs" )]
	public GameObject? DamageNumbersPrefab { get; set; }

	public event Action<int>? OnTakeDamage;
	public event Action<int>? OnHeal;
	public event Func<Task>? OnDied;

	public void TakeDamage( int damage, BattleUnit? attacker = null, Card? card = null )
	{
		if ( !GameObject.IsValid() || !IsValid )
		{
			return;
		}

		if ( damage <= 0 )
		{
			return;
		}

		var owner = Components.Get<BattleUnit>();
		if ( owner.IsValid() && owner.StatusEffects.IsValid() )
		{
			foreach ( var status in owner.StatusEffects )
			{
				damage += status.DamageModifier( card!, damage );
			}
			damage = Math.Max( damage, 0 );

			var ownerPassives = owner.Passives;
			if ( ownerPassives.IsValid() && ownerPassives.Any() )
			{
				foreach ( var passive in ownerPassives )
				{
					if ( !attacker.IsValid() )
					{
						continue;
					}

					passive.OnTakeDamage( attacker );
				}
			}

			if ( attacker.IsValid() )
			{
				var attackerPassives = attacker.Passives;
				if ( attackerPassives.IsValid() && attackerPassives.Any() )
				{
					foreach ( var passive in attackerPassives )
					{
						passive.OnDealDamage( owner );
					}
				}
			}
		}

		Health = Math.Max( Health - damage, 0 );
		OnTakeDamage?.Invoke( Health );
		DamageNumbers( damage );

		if ( Health == 0 )
		{
			_ = Die();
		}
	}

	public void TakeFixedDamage( int damage, BattleUnit? attacker = null, StatusEffect? statusEffect = null )
	{
		if ( !GameObject.IsValid() || !IsValid )
		{
			return;
		}

		if ( damage <= 0 )
		{
			return;
		}

		var owner = Components.Get<BattleUnit>();
		if ( owner.IsValid() && owner.StatusEffects.IsValid() )
		{
			var ownerPassives = owner.Passives;
			if ( ownerPassives.IsValid() && ownerPassives.Any() )
			{
				foreach ( var passive in ownerPassives )
				{
					if ( !attacker.IsValid() )
					{
						continue;
					}

					passive.OnTakeDamage( attacker );
				}
			}

			if ( attacker.IsValid() )
			{
				var attackerPassives = attacker.Passives;
				if ( attackerPassives.IsValid() && attackerPassives.Any() )
				{
					foreach ( var passive in attackerPassives )
					{
						passive.OnDealDamage( owner );
					}
				}
			}
		}

		Health = Math.Max( Health - damage, 0 );
		OnTakeDamage?.Invoke( Health );
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

	public async Task Die()
	{
		if ( OnDied is not null )
		{
			foreach ( var @delegate in OnDied.GetInvocationList() )
			{
				var handler = (Func<Task>)@delegate;
				await handler();
			}
		}
	}
}
