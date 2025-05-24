using System;
using System.Threading.Tasks;
using Sandbox.UI;
using CardGame.Platform;
using CardGame.UI;
using CardGame.UI.Tutorial;

namespace CardGame.Scripts;

public class BattleTutorial : BattleScript
{
	private static BattleManager? BattleManager => BattleManager.Instance;
	private static int TurnCount => BattleManager?.Turn ?? 0;
	
	private TaskCompletionSource<bool>? _slotSelectionTaskSource;
	private TaskCompletionSource<bool>? _cardSelectionTaskSource;
	private TaskCompletionSource<bool>? _slotAssignmentTaskSource;

	public override void OnUnload()
	{
		Log.Info( $"Cancelling tutorial script..." );
		
		_slotSelectionTaskSource?.SetCanceled();
		_cardSelectionTaskSource?.SetCanceled();
		_slotAssignmentTaskSource?.SetCanceled();
		
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
			
			if ( BattleManager.IsValid() )
			{
				BattleManager.CanEndTurn = false;
			}
			
			await DelaySeconds( 3 );
			
			TutorialPanel.Instance?.SetInputLock( true );
			await ShowMessage( "Welcome! Let's walk through your first battle.", SlotTutorial );
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
		await ShowStepMessage( "You may always choose to replay the tutorial from the main menu, or view the manual in-game for additional details not covered here" );
		await ShowStepMessage( "Good luck on your descent into the abyss!" );
		
		if ( BattleManager.IsValid() )
		{
			BattleManager.CanEndTurn = true;
		}

		TutorialPanel.Instance?.SetInputLock( false );
		if ( SceneManager.Instance.IsValid() )
		{
			BattleManager?.UnloadBattleScript();
			SceneManager.Instance.LoadScene( SceneManager.Scenes.Menu );
		}
	}

	private async Task SlotTutorial()
	{
		await ShowStepMessage( "The icons above units in battle are called card slots." );
		await ShowStepMessage( "Empty card slots are shown in gray. Filled card slots are shown in black." );
		await ShowStepMessage( "The number inside the card slot is its speed. Speed determines the order in which assigned cards are played during combat. Speed is determined individually by each card slot at turn start based on a range." );
		await ShowStepMessage( "Click on the card slot above your unit to select it.",
			position: (Length.Percent( 22 ), Length.Pixels( 500 )),
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
		await ShowStepMessage( "Selected card slots have a golden border around them." );
		await ShowStepMessage( "When you select a card slot, its owner's hand is displayed below." );
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
		await ShowStepMessage( "When you have a card selected, it will have a golden border around it." );
		await ShowStepMessage( "Cards cost a resource called ´MP´ (Mana Points) to play. You may view the amount of MP each unit has available to the right of their health value in blue." );
		await ShowStepMessage( "To queue the selected card to be played, you must select a valid target. Click on the enemy unit's card slot." );

		_slotAssignmentTaskSource = new TaskCompletionSource<bool>();
		TutorialPanel.Instance?.SetInputLock( false );
		CardSlot.OnSlotAssigned += OnSlotAssigned;

		await _slotAssignmentTaskSource.Task;

		TutorialPanel.Instance?.SetInputLock( true );
		await ShowStepMessage( "Notice the change in the background color of the card slot. Hovering over a filled card slot allows you to view the assigned card." );
		await ShowStepMessage( "If you wish to cancel your selection, right-click the filled card slot to unassign its card." );
		await TurnTutorial();
	}
	private async Task TurnTutorial()
	{
		await ShowStepMessage( "Turns are divided into two phases; the assignment phase, and the combat phase." );
		await ShowStepMessage( "During the assignment phase you may assign cards to card slots and determine your turn order." );
		await ShowStepMessage( "Once you are ready, you may either press the End Turn button at the top of the HUD or space bar to begin combat." );
		
		if ( BattleManager.IsValid() )
		{
			BattleManager.CanEndTurn = true;
		}

		TutorialPanel.Instance?.SetInputLock( false );
	}
	
	private async Task CombatTutorial()
	{
		TutorialPanel.Instance?.SetInputLock( true );
		await ShowStepMessage( "During the combat phase, cards are played in order based on their assigned slots speeds — from highest to lowest." );
		await ShowStepMessage( "If two slots share the same speed, the game will randomly decide which is played first. Enemy slots are always played first." );
		await ShowStepMessage( "Cards are then played, triggering their effects such as dealing damage, healing, or applying various status effects." );
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
