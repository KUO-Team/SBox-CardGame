using CardGame.Data;
using Sandbox.UI;
using System;
using System.Threading.Tasks;

namespace CardGame.UI;

public partial class PackOpeningPanel
{
	private static Player? Player => Player.Local;

	private Panel? _openingPanel;
	private Panel? _packAnimationPanel;
	private CardPackPanel? _packImage;
	private Panel? _particleContainer;
	private Panel? _openedCardsPanel;
	private Panel? _openedCardsContainer;

	private List<Card> _openedCards = [];
	private List<CardPanel> _cardPanels = [];
	private bool _isOpening = false;
	private bool _cardsRevealed = false;
	private bool _animationInProgress = false;
	private bool _animationComplete = false;
	private string _packClass = "";
	private CardPack? _currentPack;

	public void StartPackOpening( CardPack pack )
	{
		// Reset state
		_isOpening = true;
		_cardsRevealed = false;
		_animationInProgress = true;
		_animationComplete = false;
		_currentPack = pack;
		_cardPanels.Clear();

		_packImage?.Delete();
		_packImage = new CardPackPanel
		{
			Pack = pack
		};
		_packAnimationPanel?.AddChild(_packImage);

		// Set pack class based on rarity
		_packClass = pack.Rarity switch
		{
			CardPack.CardPackRarity.Rare => "pack-rare entrance",
			CardPack.CardPackRarity.Epic => "pack-epic entrance",
			_ => "pack-common entrance"
		};

		// Open pack but don't show cards yet
		_openedCards = pack.Open();
		StateHasChanged();

		// Run animation sequence
		_ = AnimatePackOpening();

		// Remove pack from inventory after opening
		Player?.CardPacks.Remove( pack );

		var cardPackData = PlayerData.Data.CardPacks;
		cardPackData.Remove( pack.Id );
		PlayerData.Save();
	}

	private async Task AnimatePackOpening()
	{
		await Task.Delay( 800 );

		// Run shake animations
		await ShakePackAnimation();

		// Explode pack with particles
		_packImage?.AddClass( "explode" );
		CreateParticles();
		await Task.Delay( 600 );

		// Show cards container
		_cardsRevealed = true;
		StateHasChanged();

		// Delete the card pack image.
		_packImage?.Delete();
		
		// Then reveal cards one by one
		await RevealCards();

		// Complete animation
		_animationComplete = true;
		_animationInProgress = false;
		StateHasChanged();
	}

	private async Task ShakePackAnimation()
	{
		if ( !_packImage.IsValid() )
		{
			return;
		}

		// Progressive shake sequence
		_packImage.AddClass( "shake-light" );
		await Task.Delay( 600 );
		_packImage.RemoveClass( "shake-light" );
		_packImage.AddClass( "shake-medium" );
		await Task.Delay( 500 );
		_packImage.RemoveClass( "shake-medium" );
		_packImage.AddClass( "shake" );
		await Task.Delay( 400 );
	}

	private void CreateParticles()
	{
		if ( !_particleContainer.IsValid() )
		{
			return;
		}

		for ( var i = 0; i < 30; i++ )
		{
			var particle = new Panel();
			particle.AddClass( "particle" );
			_particleContainer.AddChild( particle );

			// Randomize properties
			var angle = Game.Random.Next( 0, 360 );
			var distance = Game.Random.Next( 100, 400 );
			var size = Game.Random.Next( 3, 15 );
			var delay = Game.Random.Next( 0, 300 );
			var duration = Game.Random.Next( 800, 1500 );

			// Set particle position and style
			particle.Style.Left = Length.Percent( 50 );
			particle.Style.Top = Length.Percent( 50 );
			particle.Style.MarginLeft = Length.Pixels( -size / 2 );
			particle.Style.MarginTop = Length.Pixels( -size / 2 );
			particle.Style.Width = Length.Pixels( size );
			particle.Style.Height = Length.Pixels( size );
			particle.Style.Opacity = 0;

			// Set color based on rarity
			particle.AddClass( Game.Random.Next( 0, 100 ) switch
			{
				< 60 => "common",
				< 85 => "uncommon",
				< 97 => "rare",
				_ => "epic"
			} );

			// Calculate end position
			var radians = angle * (Math.PI / 180);
			var endX = (float)(Math.Cos( radians ) * distance);
			var endY = (float)(Math.Sin( radians ) * distance);
			var rotation = Game.Random.Next( 0, 360 );

			// Animate particle
			_ = Task.Delay( delay ).ContinueWith( task =>
			{
				particle.Style.Opacity = 1;

				var transform = new PanelTransform();
				transform.AddTranslate( endX, endY );
				transform.AddRotation( 0, 0, rotation );
				particle.Style.Transform = transform;

				_ = Task.Delay( duration ).ContinueWith( _ => particle.Style.Opacity = 0 );
			} );
		}
	}

	private async Task RevealCards()
	{
		if ( !_openedCardsPanel.IsValid() )
		{
			return;
		}

		if ( !_openedCardsContainer.IsValid() )
		{
			return;
		}
		
		foreach ( var cardPanel in _openedCardsContainer.ChildrenOfType<CardPanel>() )
		{
			_cardPanels.Add( cardPanel );
		}
		
		// Reveal each card one at a time with sequential delays
		for ( var i = 0; i < _cardPanels.Count; i++ )
		{
			var card = _cardPanels[i];
			var openedCard = i < _openedCards.Count ? _openedCards[i] : null;

			await Task.Delay( 180 );
			card.AddClass( "reveal" );

			// Add special effects after revealing
			await Task.Delay( 250 );
			card.AddClass( "shine" );

			//if ( openedCard?.Rarity >= CardPack.CardPackRarity.Epic )
			//{
			//	card.AddClass( "special-glow" );
			//}

			// Add card to player's collection
			if ( openedCard is not null )
			{
				if ( Player.IsValid() )
				{
					Player.Cards.Add( openedCard );
					PlayerData.Data.Cards.Add( openedCard.Id );
					PlayerData.Save();
				}
			}
		}

		await Task.Delay( 300 );
	}

	private void ClosePackOpening()
	{
		if ( !_animationComplete )
		{
			return;
		}

		// Reset state
		_isOpening = false;
		_cardsRevealed = false;
		_animationInProgress = false;
		_animationComplete = false;
		_currentPack = null;
		_openedCards.Clear();
		_packClass = "";

		// Clean up UI elements
		foreach ( var card in _cardPanels )
		{
			card.RemoveClass( "reveal" );
			card.RemoveClass( "shine" );
			card.RemoveClass( "special-glow" );
		}
		_cardPanels.Clear();

		// Clean up particles
		_particleContainer?.DeleteChildren( true );

		StateHasChanged();
	}

	public void Return()
	{
		this.Hide();
	}
}
