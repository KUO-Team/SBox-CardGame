using CardGame.Units;

namespace CardGame;

public class EnemyController : Component
{
	[Property, RequireComponent]
	public BattleUnit? Unit { get; set; }

	protected override void OnStart()
	{
		if ( BattleManager.Instance is not null )
		{
			BattleManager.Instance.OnTurnStart += OnTurnStart;
		}

		base.OnStart();
	}

	protected override void OnDestroy()
	{
		if ( BattleManager.Instance is not null )
		{
			BattleManager.Instance.OnTurnStart -= OnTurnStart;
		}

		base.OnDestroy();
	}

	private void OnTurnStart()
	{
		if ( !CanAct() )
		{
			return;
		}

		if ( !Unit.IsValid() || !Unit.Slots.IsValid() || !Unit.HandComponent.IsValid() )
		{
			return;
		}

		foreach ( var slot in Unit.Slots )
		{
			var randomCard = Game.Random.FromList( Unit.HandComponent.Hand! );
			if ( randomCard is null )
			{
				continue;
			}

			var oppositeFaction = Unit.Faction.GetOpposite();
			var targets = Scene.GetAllComponents<CardSlot?>()
				.Where( x => x?.Owner is not null && x.Owner.Faction == oppositeFaction )
				.ToList();

			var randomTarget = Game.Random.FromList( targets );
			if ( !randomTarget.IsValid() )
			{
				continue;
			}
			
			if ( !slot.CanAssignCard( randomCard, randomTarget ) )
			{
				continue;
			}
			
			slot.AssignCard( randomCard, randomTarget );
		}
	}

	public bool CanAct()
	{
		if ( !Unit.IsValid() )
		{
			return false;
		}

		if ( !Unit.HealthComponent.IsValid() || !Unit.Slots.IsValid() )
		{
			return false;
		}

		return !Unit.HealthComponent.IsDead && Unit.Slots.Any( x => x.IsAvailable );
	}
}
