using System;
using CardGame.StatusEffects;

namespace CardGame.Relics;

public class WitchVial( Data.Relic data ) : Relic( data )
{
	public override void OnTurnStart()
	{
		if ( !Owner.IsValid() )
		{
			return;
		}

		var (randomStatusKey, stack) = GetRandomStatusKey();
		if ( randomStatusKey is not null )
		{
			CopyStatusEffect( randomStatusKey.Value, stack );
		}

		base.OnTurnStart();
	}

	private (StatusEffect.StatusKey?, int) GetRandomStatusKey()
	{
		if ( !Owner.IsValid() )
		{
			return (null, 0);
		}

		if ( !Owner.StatusEffects.IsValid() || Owner.StatusEffects.Items.Count == 0 )
		{
			return (null, 0);
		}

		var randomStatus = Game.Random.FromList( Owner.StatusEffects.Items! );
		return (randomStatus?.Keyword, randomStatus?.Stack ?? 0);
	}

	private void CopyStatusEffect( StatusEffect.StatusKey key, int stack )
	{
		if ( !Owner.IsValid() )
		{
			return;
		}

		StatusEffect? status = key switch
		{
			StatusEffect.StatusKey.Bleed => new Bleed()
			{
				Owner = Owner, Stack = stack
			},
			StatusEffect.StatusKey.Burn => new Burn()
			{
				Owner = Owner, Stack = stack
			},
			StatusEffect.StatusKey.Enchanted => new Enchanted()
			{
				Owner = Owner, Stack = stack
			},
			StatusEffect.StatusKey.Fragile => new Fragile()
			{
				Owner = Owner, Stack = stack
			},
			StatusEffect.StatusKey.Protection => new Protection()
			{
				Owner = Owner, Stack = stack
			},
			StatusEffect.StatusKey.PowerDown => new PowerDown()
			{
				Owner = Owner, Stack = stack
			},
			StatusEffect.StatusKey.PowerUp => new PowerUp()
			{
				Owner = Owner, Stack = stack
			},
			StatusEffect.StatusKey.Immobilized => new Immobilized()
			{
				Owner = Owner, Stack = stack
			},
			StatusEffect.StatusKey.Silenced => new Silenced()
			{
				Owner = Owner, Stack = stack
			},
			StatusEffect.StatusKey.Cold => new Cold()
			{
				Owner = Owner, Stack = stack
			},
			_ => throw new ArgumentOutOfRangeException( nameof( key ), key, "Unsupported status key." )
		};

		Owner.StatusEffects?.Add( status );
	}
}
