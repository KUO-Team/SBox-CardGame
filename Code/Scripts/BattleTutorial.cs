using System;
using System.Threading.Tasks;
using CardGame.Data;
using Sandbox.UI;
using CardGame.Platform;
using CardGame.UI;
using CardGame.UI.Tutorial;
using CardGame.Units;

namespace CardGame.Scripts;

public class BattleTutorial : BattleScript
{
	private static BattleManager? BattleManager => BattleManager.Instance;
	private static int TurnCount => BattleManager?.Turn ?? 0;
	
	private TaskCompletionSource<bool>? _slotSelectionTaskSource;
	private TaskCompletionSource<bool>? _cardSelectionTaskSource;
	private TaskCompletionSource<bool>? _slotAssignmentTaskSource;

	private BattleUnitComponent? _playerUnit;
	private BattleUnitComponent? _enemyUnit;

	public override void OnUnload()
	{
		Log.Info( $"Cancelling tutorial script..." );

		try
		{
			_slotSelectionTaskSource?.SetCanceled();
			_cardSelectionTaskSource?.SetCanceled();
			_slotAssignmentTaskSource?.SetCanceled();
		}
		catch
		{
			// Ignored.
		}
		
		InputComponent.OnSelect -= OnSlotSelected;
		HandPanel.OnCardSelected -= OnCardSelected;
		CardSlot.OnSlotAssigned -= OnSlotAssigned;
		
		TutorialPanel.Instance?.SetInputLock( false );
		base.OnUnload();
	}
	
	public override async void OnTurnStart()
	{
		try
		{
			if ( TurnCount != 1 )
			{
				return;
			}
			
			_playerUnit = BattleManager.GetAliveUnits( Faction.Player ).FirstOrDefault();
			_enemyUnit = BattleManager.GetAliveUnits( Faction.Enemy ).FirstOrDefault();
			
			if ( _playerUnit is null || _enemyUnit is null )
			{
				Log.Warning( "Tutorial is missing valid units!" );
				return;
			}

			var playerSlot = _playerUnit.Slots?.FirstOrDefault();
			var enemySlot = _enemyUnit.Slots?.FirstOrDefault();
			var card = CardDataList.GetById( 9 );
			if ( card is not null )
			{
				var copy = card.DeepCopy();
				enemySlot.AssignCard( copy, playerSlot! );
			}
			
			
			if ( BattleManager.IsValid() )
			{
				BattleManager.CanEndTurn = false;
			}
			
			await DelaySeconds( 2 );
			
			TutorialPanel.Instance?.SetInputLock( true );
			await ShowMessage( "Let's walk you through your first battle.", SlotTutorial );
		}
		catch ( Exception e )
		{
			Log.Warning( $"Tutorial start failed: {e}" );
		}
	}

	public override async void OnCombatStart()
	{
		try
		{
			if ( TurnCount > 1 )
			{
				return;
			}

			await DelaySeconds( 0.5 );
			Game.ActiveScene.TimeScale = 0;
			await CombatTutorial();
		}
		catch ( Exception e )
		{
			Log.Warning( $"Failed to continue to combat tutorial; exception thrown: {e}" );
		}
	}

	public override bool CanEndBattle( Faction winner )
	{
		if ( winner == Faction.Player )
		{
			_ = Conclusion();
			return false;
		}
		
		Platform.Achievement.DieInTutorial.Unlock();
		return true;
	}

	private async Task Conclusion()
	{
		if ( !BattleManager.IsValid() )
		{
			return;
		}
		
		if ( BattleManager.Hud.IsValid() && BattleManager.Hud.TurnStartPanel.IsValid() )
		{
			BattleManager.Hud.TurnStartPanel.DontShow = true;
		}
		
		TutorialPanel.Instance?.SetInputLock( true );
		await ShowStepMessage( "This concludes the combat tutorial." );
		await ShowStepMessage( "You can replay this tutorial from the main menu or check the in-game manual for more details not covered here." );
		await ShowStepMessage( "Good luck on your descent into the abyss!" );
		
		if ( BattleManager.IsValid() )
		{
			BattleManager.CanEndTurn = true;
		}

		TutorialPanel.Instance?.SetInputLock( false );
		if ( SceneManager.Instance.IsValid() )
		{
			BattleManager?.UnloadBattleScript();
			SceneManager.Instance.LoadScene( SceneManager.Scenes.Menu, true );
		}
	}

	private async Task SlotTutorial()
	{
		await ShowStepMessage( "The icons above units in battle are called card slots." );
		await ShowStepMessage( "Empty card slots appear gray, while filled card slots appear black." );
		await ShowStepMessage( "The number inside each card slot is its speed.<br>Speed determines the order in which assigned cards are played during combat.<br>Each card slot's speed is randomly determined at turn start based on the unit's speed range." );
		await ShowStepMessage( "Click on the card slot above your unit to select it.",
			position: (Length.Percent( 24 ), Length.Pixels( 500 )),
			size: (Length.Pixels( 100 ), Length.Pixels( 100 ))
		);

		_slotSelectionTaskSource = new TaskCompletionSource<bool>();
		TutorialPanel.Instance?.SetInputLock( false );
		InputComponent.OnSelect += OnSlotSelected;

		await _slotSelectionTaskSource.Task;
		await SlotAssignmentTutorial();
	}

	private async Task SlotAssignmentTutorial()
	{
		TutorialPanel.Instance?.SetInputLock( true );
		await ShowStepMessage( "When a slot is selected, its unit’s hand appears below." );
		await ShowStepMessage( "Click on a card in the unit's hand to select it." );

		_cardSelectionTaskSource = new TaskCompletionSource<bool>();
		TutorialPanel.Instance?.SetInputLock( false );
		HandPanel.OnCardSelected += OnCardSelected;

		await _cardSelectionTaskSource.Task;
		await CardPlayTutorial();
	}

	private async Task CardPlayTutorial()
	{
		TutorialPanel.Instance?.SetInputLock( true );
		await ShowStepMessage( "Cards cost a resource called 'MP' (Mana Points) to play.<br>You can view each unit's available MP to the right of their health value, displayed in blue." );
		await ShowStepMessage( "To queue the selected card for play, you must select a valid target.<br>Click the enemy’s card slot to select it as this slot’s target." );

		_slotAssignmentTaskSource = new TaskCompletionSource<bool>();
		TutorialPanel.Instance?.SetInputLock( false );
		CardSlot.OnSlotAssigned += OnSlotAssigned;

		await _slotAssignmentTaskSource.Task;

		TutorialPanel.Instance?.SetInputLock( true );
		await ShowStepMessage( "Notice how the card slot's background color changed.<br>You can hover over any filled card slot to view its assigned card." );
		await ShowStepMessage( "If you wish to cancel your selection, right-click the filled card slot to unassign its card." );
		await TurnTutorial();
	}
	private async Task TurnTutorial()
	{
		await ShowStepMessage( "Turns are divided into two phases: the assignment phase and the combat phase." );
		await ShowStepMessage( "During the assignment phase, you can assign cards to card slots and determine your turn order." );
		await ShowStepMessage( "When you're ready, press either the End Turn button at the top of the HUD or the spacebar key to switch to the combat phase." );
		
		if ( BattleManager.IsValid() )
		{
			BattleManager.CanEndTurn = true;
		}

		TutorialPanel.Instance?.SetInputLock( false );
	}
	
	private async Task CombatTutorial()
	{
		TutorialPanel.Instance?.SetInputLock( true );
		await ShowStepMessage( "During the combat phase, cards are played in order based on their assigned slot's speed — from highest to lowest." );
		await ShowStepMessage( "If two or more slots share the same speed, one is chosen randomly to play first.<br><br>However, enemy slots are always played before player slots when speeds are tied." );
		await ShowStepMessage( "Cards then activate, triggering their effects such as dealing damage or causing various effects." );
		await ShowStepMessage( "Let's finish this fight! Use what you've learned to defeat the enemy." );

		Game.ActiveScene.TimeScale = 1;
		TutorialPanel.Instance?.SetInputLock( false );
	}

	private void OnSlotSelected( CardSlot slot )
	{
		InputComponent.OnSelect -= OnSlotSelected;

		if ( _slotSelectionTaskSource is { Task.IsCompleted: false } )
		{
			_slotSelectionTaskSource.SetResult( true );
		}
		else
		{
			Log.Warning( "Slot selection task source was null or already completed" );
		}
	}

	private void OnCardSelected( Card card )
	{
		HandPanel.OnCardSelected -= OnCardSelected;

		if ( _cardSelectionTaskSource is { Task.IsCompleted: false } )
		{
			_cardSelectionTaskSource.SetResult( true );
		}
		else
		{
			Log.Warning( "Card selection task source was null or already completed" );
		}
	}

	private void OnSlotAssigned( CardSlot slot, Card card, CardSlot target )
	{
		CardSlot.OnSlotAssigned -= OnSlotAssigned;

		if ( card.IsInstant )
		{
			
		}

		if ( _slotAssignmentTaskSource is { Task.IsCompleted: false } )
		{
			_slotAssignmentTaskSource.SetResult( true );
		}
		else
		{
			Log.Warning( "Slot assignment task source was null or already completed" );
		}
	}

	public static Task ShowStepMessage( string message, (Length? Left, Length? Top)? position = null, (Length? Width, Length? Height)? size = null )
	{
		var tcs = new TaskCompletionSource<bool>();

		var onFinish = () =>
		{
			tcs.SetResult( true );
			return Task.CompletedTask;
		};

		var shouldHighlight = position.HasValue || size.HasValue;
		if ( shouldHighlight )
		{
			_ = ShowMessageWithHighlight( message, onFinish, position: position, size: size );
		}
		else
		{
			_ = ShowMessage( message, onFinish );
		}

		return tcs.Task;
	}

	private static Task ShowMessage( string message, Func<Task>? onFinish = null )
	{
		if ( !TutorialPanel.Instance.IsValid() )
		{
			Log.Warning( "Unable to play message; no tutorial panel found!" );
			return Task.CompletedTask;
		}

		var completion = new TaskCompletionSource();

		TutorialPanel.Instance.ShowInfo( message, async void () =>
		{
			try
			{
				await DelaySeconds( 0.1f );
				if ( onFinish is not null )
				{
					await onFinish();
				}
			}
			catch ( Exception e )
			{
				TutorialPanel.Instance?.SetInputLock( false );
				Log.Warning( $"Tutorial highlight failed: {e}" );
			}
			finally
			{
				completion.SetResult();
			}
		} );

		return completion.Task;
	}

	private static Task ShowMessageWithHighlight( string message, Func<Task>? onFinish = null, (Length? Left, Length? Top)? position = null, (Length? Width, Length? Height)? size = null )
	{
		if ( !TutorialPanel.Instance.IsValid() )
		{
			Log.Warning( "Unable to show highlight; no tutorial panel found!" );
			return Task.CompletedTask;
		}

		var completion = new TaskCompletionSource();

		TutorialPanel.Instance.ShowInfo( message, async void () =>
		{
			try
			{
				await DelaySeconds( 0.1f );
				if ( onFinish is not null )
				{
					await onFinish();
				}
			}
			catch ( Exception e )
			{
				TutorialPanel.Instance?.SetInputLock( false );
				Log.Warning( $"Tutorial highlight failed: {e}" );
			}
			finally
			{
				completion.SetResult();
			}
		}, position, size );

		return completion.Task;
	}

	private static async Task DelaySeconds( double seconds )
	{
		await Task.Delay( (int)(seconds * 1000) );
	}
}
