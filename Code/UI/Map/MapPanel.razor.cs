using System;
using Sandbox.UI;
using Sandbox.Diagnostics;
using CardGame.Data;

namespace CardGame.UI.Map;

public partial class MapPanel
{
	public Panel? Map { get; set; }

	private Random _seededRandom = new();

	public RelicGainPanel? RelicGainPanel { get; set; }
	public EventPanel? EventPanel { get; set; }

	private static int CurrentNodeIndex
	{
		get
		{
			return MapManager?.Index ?? 0;
		}
	}

	private List<MapNode> Nodes => Map?.ChildrenOfType<MapNode>().OrderBy( n => n.Index ).ToList() ?? [];
	private readonly List<MapNode.MapNodeType> _nodeTypes = [];
	private readonly List<Vector2> _nodePositions = [];
	private readonly List<MapConnection> _mapConnections = [];
	private readonly Dictionary<int, Id> _events = new();

	private static GameManager? GameManager => GameManager.Instance;
	private static MapManager? MapManager => MapManager.Instance;
	private static SaveManager? SaveManager => SaveManager.Instance;
	private static RelicManager? RelicManager => RelicManager.Instance;

	private static readonly Logger Log = new( "Map" );

	private BattleInfoPanel? _battleInfoPanel;
	private ShopPanel? _shopPanel;
	private Label? _mapIndicator;

	private readonly Queue<Relics.Relic> _relics = [];

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
			Text = "\u25bc",
			Classes = "indicator"
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

	public void GenerateMapLayout( int? seed = null )
	{
		if ( !GameManager.IsValid() || !Map.IsValid() || !MapManager.IsValid() )
		{
			Log.Warning( "Unable to generate map; invalid state." );
			return;
		}

		Log.Info( seed.HasValue ? $"Generating map layout with seed {seed.Value}" : "Generating random map layout" );

		_nodePositions.Clear();
		_nodeTypes.Clear();
		_mapConnections.Clear();

		_seededRandom = seed.HasValue ? new Random( seed.Value ) : new Random();
		var totalTiers = MapManager.GetTierCount();
		var maxNodesPerTier = MapManager.GetMaxNodesPerTier();

		var tiers = GenerateTiers( totalTiers, maxNodesPerTier );
		InjectShopNode( tiers );
		InjectEventNodes( tiers );

		_nodeTypes[tiers.First()[0]] = MapNode.MapNodeType.Start;
		_nodeTypes[tiers.Last()[0]] = MapNode.MapNodeType.Boss;

		Map.DeleteChildren();

		for ( var i = 0; i < _nodePositions.Count; i++ )
		{
			var node = CreateMapNode( i );
			Map?.AddChild( node );
		}

		MapManager.MapGenerated = true;
		MapManager.Index = 0;
		GenerateConnections( tiers );
	}

	private List<List<int>> GenerateTiers( int totalTiers, int maxNodesPerTier )
	{
		var tiers = new List<List<int>>();
		var nodeIndex = 0;

		for ( var tier = 0; tier < totalTiers; tier++ )
		{
			var nodesInTier = (tier == 0 || tier == totalTiers - 1) ? 1 : _seededRandom.Next( 1, maxNodesPerTier + 1 );
			var x = (float)tier / (totalTiers - 1) * 100f;
			var tierList = new List<int>();

			for ( var j = 0; j < nodesInTier; j++ )
			{
				var y = (float)(j + 1) / (nodesInTier + 1) * 100f;
				_nodePositions.Add( new Vector2( x, y ) );
				_nodeTypes.Add( MapNode.MapNodeType.Battle );
				tierList.Add( nodeIndex++ );
			}
			tiers.Add( tierList );
		}
		return tiers;
	}

	private void InjectShopNode( List<List<int>> tiers )
	{
		if ( tiers.Count <= 2 )
		{
			return;
		}

		// Skip tier 1 entirely — start from tier 2
		var eligibleTiers = Enumerable.Range( 2, tiers.Count - 3 ).ToList();
		var randomTier = eligibleTiers[_seededRandom.Next( eligibleTiers.Count )];
		var candidates = tiers[randomTier];
		if ( candidates.Count == 0 )
		{
			return;
		}

		var shopIndex = candidates[_seededRandom.Next( candidates.Count )];
		_nodeTypes[shopIndex] = MapNode.MapNodeType.Shop;
	}

	private void InjectEventNodes( List<List<int>> tiers )
	{
		if ( !GameManager.IsValid() )
		{
			return;
		}

		if ( !MapManager.IsValid() )
		{
			return;
		}

		if ( !MapManager.FloorEvents.TryGetValue( GameManager.Floor, out var events ) || events.Count == 0 || tiers.Count <= 3 )
		{
			return;
		}

		var availableEvents = new Queue<Id>( events.OrderBy( _ => _seededRandom.Next() ) );

		// Exclude tier 1
		var middleTiers = Enumerable.Range( 2, tiers.Count - 3 ).OrderBy( _ => _seededRandom.Next() );

		foreach ( var tierIndex in middleTiers )
		{
			if ( availableEvents.Count == 0 )
			{
				break;
			}

			var candidates = tiers[tierIndex].Where( i => _nodeTypes[i] == MapNode.MapNodeType.Battle ).ToList();
			if ( candidates.Count == 0 )
			{
				continue;
			}

			var nodeIndex = candidates[_seededRandom.Next( candidates.Count )];
			_nodeTypes[nodeIndex] = MapNode.MapNodeType.Event;

			// Sneaky trick: cache the event ID in the node itself via node.Event later
			// Queue ensures each event gets used exactly once
			var selectedEvent = availableEvents.Dequeue();
			_events[nodeIndex] = selectedEvent;
		}
	}

	private MapNode? CreateMapNode( int index )
	{
		if ( !GameManager.IsValid() )
		{
			return null;
		}

		if ( !MapManager.IsValid() )
		{
			return null;
		}

		var type = _nodeTypes[index];
		var position = _nodePositions[index];
		var node = new MapNode
		{
			Index = index,
			Type = type,
			Style =
			{
				Left = Length.Percent( position.x ), Top = Length.Percent( position.y )
			}
		};

		switch ( type )
		{
			case MapNode.MapNodeType.Battle:
				node.Battle = Game.Random.FromList( MapManager.FloorBattles[GameManager.Floor]! );
				break;
			case MapNode.MapNodeType.Event when _events.TryGetValue( index, out var eventId ):
				node.Event = eventId;
				break;
			case MapNode.MapNodeType.Start:
			case MapNode.MapNodeType.Shop:
			case MapNode.MapNodeType.Boss:
				break;
			default:
				throw new ArgumentOutOfRangeException( type.ToString() );
		}

		node.AddEventListener( "onclick", () => Select( index ) );
		node.AddEventListener( "onmouseout", OnNodeUnhover );
		node.AddEventListener( "onmouseover", () => OnNodeHover( index ) );

		return node;
	}

	private void GenerateConnections( List<List<int>> tiers )
	{
		for ( var i = 0; i < tiers.Count - 1; i++ )
		{
			var current = tiers[i];
			var next = tiers[i + 1];

			foreach ( var to in next )
			{
				var from = current[_seededRandom.Next( current.Count )];
				AddConnection( from, to );
			}

			foreach ( var from in current )
			{
				var shuffled = next.OrderBy( _ => _seededRandom.Next() ).ToList();
				var extra = _seededRandom.Next( 0, next.Count + 1 );
				for ( var j = 0; j < extra; j++ )
				{
					var to = shuffled[j];
					if ( !_mapConnections.Any( c => c.From == from && c.To == to ) )
					{
						AddConnection( from, to );
					}
				}
			}

			foreach ( var from in current )
			{
				if ( _mapConnections.Any( c => c.From == from ) )
				{
					continue;
				}

				var to = next[_seededRandom.Next( next.Count )];
				AddConnection( from, to );
			}
		}
	}

	private void AddConnection( int from, int to )
	{
		_mapConnections.Add( new MapConnection( from, to, null! ) );
	}

	public void DeleteLayout()
	{
		if ( !Map.IsValid() || !MapManager.IsValid() )
		{
			return;
		}

		MapManager.MapGenerated = false;
		Map.DeleteChildren( true );
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

		foreach ( var conn in _mapConnections )
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

		foreach ( var conn in _mapConnections )
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
		var validNextNodes = _mapConnections
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

	private int _hoveredNode = -1;

	public void OnNodeHover( int index )
	{
		_hoveredNode = index;

		foreach ( var conn in _mapConnections.Where( conn => conn.From == _hoveredNode ) )
		{
			conn.LinePanel.AddClass( "highlighted" );
		}
	}

	public void OnNodeUnhover()
	{
		_hoveredNode = -1;

		foreach ( var conn in _mapConnections )
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
		var validNextNodes = _mapConnections
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
		RunNode( Nodes[index] );
	}

	private void RunNode( MapNode node )
	{
		if ( !MapManager.IsValid() || !GameManager.IsValid() )
		{
			return;
		}

		switch ( node.Type )
		{
			case MapNode.MapNodeType.Battle:
				{
					if ( node.Battle is null )
					{
						Log.Error( $"No battle set on node {node}!" );
						return;
					}

					var battle = BattleDataList.GetById( node.Battle );
					if ( battle is null )
					{
						return;
					}

					_battleInfoPanel?.Delete();
					_battleInfoPanel = new BattleInfoPanel
					{
						Battle = battle
					};

					Panel.AddChild( _battleInfoPanel );
					break;
				}
			case MapNode.MapNodeType.Shop:
				{
					ShowShop();
					node.Completed = true;
					break;
				}
			case MapNode.MapNodeType.Start:
				break;
			case MapNode.MapNodeType.Boss:
				{
					if ( !MapManager.FloorBosses.TryGetValue( GameManager.Floor, out var battles ) )
					{
						Log.Error( $"No bosses set on floor {GameManager.Floor}!" );
						return;
					}

					var randomBattle = Game.Random.FromList( battles! );
					if ( randomBattle is not null )
					{
						var battle = BattleDataList.GetById( randomBattle );
						if ( battle is null )
						{
							return;
						}

						_battleInfoPanel?.Delete();
						_battleInfoPanel = new BattleInfoPanel
						{
							Battle = battle
						};

						Panel.AddChild( _battleInfoPanel );
					}
					break;
				}
			case MapNode.MapNodeType.Event:
				{
					if ( node.Event is null )
					{
						Log.Error( $"No event set on node {node}!" );
						return;
					}

					if ( !EventPanel.IsValid() )
					{
						Log.Error( $"Unable to show event; no event panel set!" );
						return;
					}
					EventPanel.ShowEventById( node.Event );
					node.Completed = true;
					break;
				}
			default:
				throw new ArgumentOutOfRangeException( node.Type.ToString() );
		}
	}

	public void ShowShop()
	{
		if ( !_shopPanel.IsValid() )
		{
			return;
		}

		_shopPanel.Show();
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Player.Local?.Money, Player.Local?.Unit?.Hp );
	}
}
