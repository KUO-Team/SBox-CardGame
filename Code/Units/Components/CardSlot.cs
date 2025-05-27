using System;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using CardGame.Units;
using CardGame.Effects;
using CardGame.UI;
using Sandbox.UI;

namespace CardGame;

public sealed class CardSlot : Component, IOwnable
{
	[Property, Category( "Components" ), RequireComponent]
	public BattleUnit? Owner { get; set; }

	[Property, JsonIgnore, ReadOnly]
	public Card? AssignedCard { get; set; }

	[Property, JsonIgnore, ReadOnly]
	public bool IsAssigned => AssignedCard is not null;

	[Property, JsonIgnore, ReadOnly]
	public CardSlot? Target { get; set; }

	[Property]
	public bool IsAvailable { get; set; } = true;

	[Property]
	public int MinSpeed { get; set; } = 1;

	[Property]
	public int MaxSpeed { get; set; } = 6;

	[Property]
	public int BaseSpeed { get; set; }

	[Property]
	public int Speed { get; set; }
	
	[Property, Category( "Components" )]
	public LineRenderer? LineRenderer { get; set; }

	public CardSlotPanel? Panel { get; set; }

	public static Action<CardSlot, Card, CardSlot>? OnSlotAssigned { get; set; }

	private Ray _mouseRay;

	protected override void OnStart()
	{
		SetRandomSpeed();
		if ( BattleManager.Instance.IsValid() )
		{
			BattleManager.Instance.OnTurnStart += OnTurnStart;
			BattleManager.Instance.OnCombatStart += OnCombatStart;
		}

		LineRenderer = Components.GetInChildren<LineRenderer>();
		base.OnStart();
	}

	protected override void OnDestroy()
	{
		if ( BattleManager.Instance.IsValid() )
		{
			BattleManager.Instance.OnTurnStart -= OnTurnStart;
			BattleManager.Instance.OnCombatStart -= OnCombatStart;
		}

		foreach ( var @delegate in OnSlotAssigned?.GetInvocationList() ?? [] )
		{
			OnSlotAssigned -= @delegate as Action<CardSlot, Card, CardSlot>;
		}

		base.OnDestroy();
	}

	protected override void OnUpdate()
	{
		if ( InputComponent.SelectedSlot != this )
		{
			return;
		}

		if ( IsAssigned )
		{
			return;
		}
		
		if ( !Panel.IsValid() )
		{
			return;
		}
		
		var mousePosition = Mouse.Position;
		_mouseRay = Scene.Camera.ScreenPixelToRay( mousePosition );
		
		var slotPosition = WorldPosition;
		var plane = new Plane( Vector3.Up, 0 );
		var rayStart = _mouseRay.Position;
		var rayEnd = _mouseRay.Position + _mouseRay.Forward * 10000f;
		
		var hit = plane.IntersectLine( rayStart, rayEnd );
		if ( hit.HasValue )
		{
			var cursorWorldPosition = hit.Value;
			DrawTargetingArrows( slotPosition, cursorWorldPosition );
		}
	}
	
	private void OnTurnStart()
	{
		SetRandomSpeed();
	}

	private void OnCombatStart()
	{
		var battleManager = BattleManager.Instance;
		if ( battleManager.IsValid() )
		{
			var hud = battleManager.Hud;
			if ( hud.IsValid() )
			{
				foreach ( var handPanel in hud.Panel.ChildrenOfType<HandPanel>() )
				{
					handPanel.Hide();
				}
			}
		}
		
		if ( IsAssigned )
		{
			return;
		}

		ClearTargetingArrows();
	}

	public void SetRandomSpeed()
	{
		BaseSpeed = Game.Random.Int( MinSpeed, MaxSpeed );
		Speed = BaseSpeed;
	}

	public void AssignCard( Card card, CardSlot target )
	{
		if ( !CanAssignCard( card, target ) )
		{
			return;
		}

		if ( !Owner.IsValid() )
		{
			return;
		}

		Owner.Energy -= card.EffectiveCost.Ep;
		Owner.Mana -= card.EffectiveCost.Mp;
		AssignedCard = card;
		Target = target;

		var hand = Owner.HandComponent;
		hand?.Hand.Remove( AssignedCard );

		if ( card.IsInstant && target.Owner.IsValid() )
		{
			card.Play( target.Owner, this );
		}

		OnSlotAssigned?.Invoke( this, card, target );
		HandPanel.SelectedCard = null;

		if ( !Panel.IsValid() )
		{
			return;
		}

		var slotPosition = WorldPosition;
		var targetPosition = Target.WorldPosition;
		DrawTargetingArrows( slotPosition.WithZ( 50 ), targetPosition.WithZ( 50 ) );
	}

	public void UnassignCard()
	{
		if ( !IsAssigned )
		{
			return;
		}

		if ( !Owner.IsValid() )
		{
			return;
		}

		Owner.Energy += AssignedCard!.EffectiveCost.Ep;
		Owner.Mana += AssignedCard.EffectiveCost.Mp;
		var hand = Owner.HandComponent;
		hand?.Hand.Add( AssignedCard );
		AssignedCard = null;

		Log.Info( $"Unassigned card from slot {this}." );
		ClearTargetingArrows();
	}

	public bool CanAssignCard( Card card, CardSlot target )
	{
		if ( IsAssigned )
		{
			return false;
		}

		if ( !Owner.IsValid() || !target.IsValid() )
		{
			return false;
		}

		if ( !IsAvailable )
		{
			return false;
		}

		if ( Owner.Energy < card.EffectiveCost.Ep )
		{
			return false;
		}

		if ( Owner.Mana < card.EffectiveCost.Mp )
		{
			return false;
		}

		foreach ( var action in card.Actions )
		{
			if ( action.Effect is not {} effect )
			{
				continue;
			}

			var detail = new CardEffect.CardEffectDetail
			{
				Unit = Owner, Target = target.Owner
			};

			if ( !effect.CanPlay( detail ) )
			{
				return false;
			}
		}

		return true;
	}

	public async Task PlayAsync( BattleUnit target, CardSlot slot )
	{
		if ( !IsAssigned )
		{
			return;
		}

		ClearTargetingArrows();
		slot.AssignedCard!.Play( target, slot );
		await Task.DelaySeconds( GetCardAnimationDuration() );
	}

	public float GetCardAnimationDuration()
	{
		if ( !HasAnimations() )
		{
			return 0;
		}
		
		// TODO: Wait for animations.
		return 1f;
	}

	private bool HasAnimations()
	{
		if ( !Owner.IsValid() )
		{
			return false;
		}

		if ( !Owner.HealthComponent.IsValid() || Owner.HealthComponent.IsDead )
		{
			return false;
		}

		return true;
	}
	
	public void DrawTargetingArrows( Vector3 start, Vector3 end )
	{
		if ( !LineRenderer.IsValid() )
		{
			Log.Warning( $"Can't update targeting arrows; no line renderer found!" );
			return;
		}

		ClearTargetingArrows();
		LineRenderer.VectorPoints.Add( start );
		LineRenderer.VectorPoints.Add( end );
	}

	public void ClearTargetingArrows()
	{
		if ( !LineRenderer.IsValid() )
		{
			Log.Warning( $"Can't update targeting arrows; no line renderer found!" );
			return;
		}

		LineRenderer.VectorPoints.Clear();
	}
}
