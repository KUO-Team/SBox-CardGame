using System;
using CardGame.StatusEffects;
using CardGame.Units;

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

		var status = StatusEffectList.CreateStatusEffectByKey( key );
		if ( status is null )
		{
			return;
		}
		
		Owner.StatusEffects?.AddStatusEffect( status, stack );
	}
}
