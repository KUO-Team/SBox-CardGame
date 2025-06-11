using System;
using Sandbox.UI;
using Sandbox.Diagnostics;
using CardGame.Data;

namespace CardGame.UI.Map;

public partial class MapPanel
{
	public Panel? Map { get; set; }

	public RelicGainPanel? RelicGainPanel { get; set; }
	public EventPanel? EventPanel { get; set; }

	private static int CurrentNodeIndex
	{
		get
		{
			return MapManager?.Index ?? 0;
		}
	}

	private Random _seededRandom = new();
	private readonly Dictionary<int, Id> _events = new();
	private readonly Queue<Relics.Relic> _relics = [];

	private static GameManager? GameManager => GameManager.Instance;
	private static MapManager? MapManager => MapManager.Instance;
	private static SaveManager? SaveManager => SaveManager.Instance;
	private static RelicManager? RelicManager => RelicManager.Instance;

	private static readonly Logger Log = new( "Map" );

	private ShopPanel? _shopPanel;
	private Label? _mapIndicator;
	private Panel? _mapKey;

	private int _hoveredNode = -1;

	protected override void OnTreeFirstBuilt()
	{
		if ( !MapManager.IsValid() || !SaveManager.IsValid() || !GameManager.IsValid() || !RelicManager.IsValid() )
		{
			return;
		}

		GenerateMapLayout( MapManager.Seed );
		if ( SaveManager.ActiveRunData is not null )
		{
			MapManager.Index = SaveManager.ActiveRunData.MapNodeIndex;

			foreach ( var index in SaveManager.ActiveRunData.CompletedNodes.ToArray() )
			{
				if ( index < 0 || index >= Nodes.Count )
				{
					continue;
				}

				var node = Nodes[index];
				node.Completed = true;

				var isFinalNode = index == Nodes.Last().Index;
				if ( !isFinalNode )
				{
					continue;
				}

				if ( GameManager.Floor == 0 )
				{
					GameManager.EndRunInWin();
				}
				else
				{
					MapManager.GenerateNewFloor();
				}
			}
		}

		_mapIndicator = new Label
		{
			Text = "\u25bc", Classes = "indicator"
		};
		Map?.AddChild( _mapIndicator );

		foreach ( var relic in RelicManager.Relics.Where( x => !RelicManager.ShownRelics.Contains( x ) ) )
		{
			_relics.Enqueue( relic );
		}

		if ( RelicGainPanel.IsValid() && _relics.Count > 0 )
		{
			ShowNextRelic();
		}

		RenderLines();

		base.OnTreeFirstBuilt();
	}

	protected override void OnTreeBuilt()
	{
		UpdateNodeStyles();
		UpdateNodeStates();
		UpdateLines();
		UpdateMapIndicator();

		base.OnTreeBuilt();
	}

	private void UpdateMapIndicator()
	{
		if ( !_mapIndicator.IsValid() )
		{
			return;
		}

		if ( CurrentNodeIndex < 0 || CurrentNodeIndex >= Nodes.Count )
		{
			_mapIndicator.Style.Display = DisplayMode.None;
			return;
		}

		if ( !Map.IsValid() )
		{
			return;
		}

		var currentNodePosition = _nodePositions[CurrentNodeIndex];
		_mapIndicator.Style.Left = Length.Percent( currentNodePosition.x + 0.1f );
		_mapIndicator.Style.Top = Length.Percent( currentNodePosition.y - 6 );
	}

	private void ShowNextRelic()
	{
		if ( !RelicManager.IsValid() )
		{
			return;
		}

		if ( !RelicGainPanel.IsValid() )
		{
			return;
		}

		if ( _relics.Count == 0 )
		{
			RelicGainPanel.Hide();
			return;
		}

		var relic = _relics.Dequeue();
		RelicManager.ShownRelics.Add( relic );
		RelicGainPanel.Show( relic.Data, ShowNextRelic );
	}

	public void ShowShop()
	{
		if ( !_shopPanel.IsValid() )
		{
			return;
		}

		_shopPanel.Show();
	}

	private void ToggleMapKey()
	{
		if ( !_mapKey.IsValid() )
		{
			return;
		}

		if ( _mapKey.HasClass( "hidden" ) )
		{
			_mapKey.Show();
		}
		else
		{
			_mapKey.Hide();
		}
	}

	private void RenderLines()
	{
		if ( !Map.IsValid() )
		{
			return;
		}

		Map.ChildrenOfType<Panel>()
			.Where( p => p.HasClass( "map-line" ) )
			.ToList()
			.ForEach( p => p.Delete() );

		foreach ( var conn in MapConnections )
		{
			var linePanel = new Panel();
			linePanel.AddClass( "map-line" );
			Map.AddChild( linePanel );
			conn.LinePanel = linePanel;
		}
	}

	private void UpdateLines()
	{
		if ( !Map.IsValid() )
		{
			return;
		}

		foreach ( var conn in MapConnections )
		{
			var mapWidth = Map.Box.Rect.Width;
			var mapHeight = Map.Box.Rect.Height;

			if ( mapWidth <= 0 || mapHeight <= 0 )
			{
				continue;
			}

			var fromPos = _nodePositions[conn.From];
			var toPos = _nodePositions[conn.To];

			var fromPixelX = (fromPos.x / 100f) * mapWidth;
			var fromPixelY = (fromPos.y / 100f) * mapHeight;
			var toPixelX = (toPos.x / 100f) * mapWidth;
			var toPixelY = (toPos.y / 100f) * mapHeight;
			var dx = toPixelX - fromPixelX;
			var dy = toPixelY - fromPixelY;
			var pixelDistance = MathF.Sqrt( dx * dx + dy * dy );
			var angle = MathF.Atan2( dy, dx ) * (180 / MathF.PI);
			var percentageDistance = (pixelDistance / mapWidth) * 100f;

			var linePanel = conn.LinePanel;
			if ( !linePanel.IsValid() )
			{
				continue;
			}

			linePanel.Style.Left = Length.Percent( fromPos.x );
			linePanel.Style.Top = Length.Percent( fromPos.y );
			linePanel.Style.Width = Length.Percent( percentageDistance );
			linePanel.Style.Set( "transform-origin", "0 0" );
			linePanel.Style.Set( "transform", $"rotate({angle}deg)" );
		}
	}

	private void UpdateNodeStyles()
	{
		for ( var i = 0; i < Nodes.Count; i++ )
		{
			Nodes[i].Style.Left = Length.Percent( _nodePositions[i].x );
			Nodes[i].Style.Top = Length.Percent( _nodePositions[i].y );
		}
	}

	private void UpdateNodeStates()
	{
		var validNextNodes = MapConnections
			.Where( c => c.From == CurrentNodeIndex )
			.Select( c => c.To )
			.ToHashSet();

		for ( var i = 0; i < Nodes.Count; i++ )
		{
			var node = Nodes[i];
			var isEnabled = i == CurrentNodeIndex || IsCurrentNodeCompleted() && validNextNodes.Contains( i );
			node.Enabled = isEnabled;
			node.SetClass( "disabled", !isEnabled );
			node.SetClass( "current", i == CurrentNodeIndex );
		}
	}

	public void OnNodeHover( int index )
	{
		_hoveredNode = index;

		foreach ( var conn in MapConnections.Where( conn => conn.From == _hoveredNode ) )
		{
			conn.LinePanel.AddClass( "highlighted" );
		}
	}

	public void OnNodeUnhover()
	{
		_hoveredNode = -1;

		foreach ( var conn in MapConnections )
		{
			conn.LinePanel.RemoveClass( "highlighted" );
		}
	}

	public void Select( int index )
	{
		if ( index < 0 || index >= Nodes.Count )
		{
			Log.Warning( $"Invalid node index: {index}." );
			return;
		}

		if ( !MapManager.IsValid() )
		{
			return;
		}

		if ( !SaveManager.IsValid() || SaveManager.ActiveRunData is null )
		{
			return;
		}

		if ( !CanSelectNode( index ) )
		{
			Log.EditorLog( $"Cannot move to node {index} from {CurrentNodeIndex} [{Nodes[index].Type}]" );
			return;
		}

		ProceedToNode( index );
	}

	private bool CanSelectNode( int index )
	{
		var validNextNodes = MapConnections
			.Where( c => c.From == CurrentNodeIndex )
			.Select( c => c.To )
			.ToHashSet();

		if ( index == CurrentNodeIndex )
		{
			// Can only click the current node if it's not done
			return !IsCurrentNodeCompleted();
		}

		// Otherwise, only allow moving forward if current node is completed
		return IsCurrentNodeCompleted() && validNextNodes.Contains( index ) && index >= 0 && index < Nodes.Count;
	}

	private bool IsCurrentNodeCompleted()
	{
		if ( CurrentNodeIndex < 0 || CurrentNodeIndex >= Nodes.Count )
		{
			return false;
		}

		var node = Nodes[CurrentNodeIndex];
		return node.Type == MapNode.MapNodeType.Start || node.Completed;
	}

	private void ProceedToNode( int index )
	{
		if ( !MapManager.IsValid() )
		{
			return;
		}

		MapManager.Index = index;
		if ( SaveManager.IsValid() && SaveManager.ActiveRunData is not null )
		{
			SaveManager.ActiveRunData.MapNodeIndex = index;
		}

		Log.Info( $"Selected node at index {index}" );

		var node = Nodes[index];
		node.Run( this );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Player.Local?.Money, Player.Local?.Unit?.Hp );
	}
}
