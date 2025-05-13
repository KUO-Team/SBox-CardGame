using System;
using Sandbox.UI;
using Sandbox.Diagnostics;
using CardGame.Data;

namespace CardGame.UI.Map;

public partial class MapPanel
{
	public Panel? Map { get; set; }

	private Random _seededRandom = new();

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

	private static GameManager? GameManager => GameManager.Instance;
	private static MapManager? MapManager => MapManager.Instance;
	private static SaveManager? SaveManager => SaveManager.Instance;

	private static readonly Logger Log = new( "Map" );

	private BattleInfoPanel? _battleInfoPanel;
	private EventPanel? _eventPanel;
	private ShopPanel? _shopPanel;

	protected override void OnTreeFirstBuilt()
	{
		if ( !MapManager.IsValid() || !SaveManager.IsValid() )
		{
			return;
		}

		GenerateMapLayout( MapManager.Seed );
		if ( SaveManager.ActiveRunData is not null )
		{
			// Restore the saved node position
			MapManager.Index = SaveManager.ActiveRunData.MapNodeIndex;
		}

		base.OnTreeFirstBuilt();
	}

	protected override void OnTreeBuilt()
	{
		if ( !GameManager.IsValid() || !MapManager.IsValid() || !SaveManager.IsValid() )
		{
			return;
		}

		Update();
		UpdateNodeStates();

		if ( SaveManager.ActiveRunData is not null )
		{
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
					StateHasChanged();
					OnTreeBuilt();
				}
			}

			Update();
		}

		base.OnTreeBuilt();
	}

	private Vector2 GetPixelPosition( Vector2 percentPos )
	{
		if ( !Map.IsValid() )
		{
			return Vector2.Zero;
		}

		var width = Map.Box.Rect.Width;
		var height = Map.Box.Rect.Height;
		var x = percentPos.x / 100f * width;
		var y = percentPos.y / 100f * height;

		return new Vector2( x, y );
	}

	public void GenerateMapLayout( int? seed = null )
	{
		if ( !Map.IsValid() )
		{
			Log.Warning( "Unable to generate map; no valid map!" );
			return;
		}

		if ( !MapManager.IsValid() )
		{
			Log.Warning( "Unable to generate map; no valid map manager!" );
			return;
		}

		if ( seed.HasValue )
		{
			Log.Info( $"Generating map layout with seed {seed.Value}" );
		}
		else
		{
			Log.Info( $"Generating random map layout" );
		}

		_nodePositions.Clear();
		_nodeTypes.Clear();
		_mapConnections.Clear();

		_seededRandom = seed.HasValue ? new Random( seed.Value ) : new Random();

		var totalTiers = MapManager.GetTierCount();
		var maxNodesPerTier = MapManager.GetMaxNodesPerTier();

		var tiers = new List<List<int>>();
		var nodeIndex = 0;

		for ( var tier = 0; tier < totalTiers; tier++ )
		{
			var nodesInTier = (tier == 0 || tier == totalTiers - 1)
				? 1
				: _seededRandom.Next( 1, maxNodesPerTier + 1 );

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

		// Inject at least one shop node in a random middle tier
		if ( totalTiers > 2 )
		{
			var eligibleTiers = Enumerable.Range( 1, totalTiers - 2 ).ToList();
			var randomTier = eligibleTiers[_seededRandom.Next( eligibleTiers.Count )];
			var shopCandidates = tiers[randomTier];
			if ( shopCandidates.Count > 0 )
			{
				var shopNodeIndex = shopCandidates[_seededRandom.Next( shopCandidates.Count )];
				_nodeTypes[shopNodeIndex] = MapNode.MapNodeType.Shop;
			}
		}

		// Inject a few event nodes randomly in middle tiers
		if ( totalTiers > 3 )
		{
			var eventTierIndices = Enumerable.Range( 1, totalTiers - 2 ).OrderBy( _ => _seededRandom.Next() ).Take( 2 );

			foreach ( var eventTier in eventTierIndices )
			{
				var candidates = tiers[eventTier];
				if ( candidates.Count <= 0 )
				{
					continue;
				}
				
				var eventNodeIndex = candidates[_seededRandom.Next( candidates.Count )];

				// Only override battles.
				if ( _nodeTypes[eventNodeIndex] == MapNode.MapNodeType.Battle )
				{
					_nodeTypes[eventNodeIndex] = MapNode.MapNodeType.Event;
				}
			}
		}

		Map.DeleteChildren();

		// Set the first node
		var firstNodeIndex = tiers.First()[0];
		_nodeTypes[firstNodeIndex] = MapNode.MapNodeType.Start;

		// Set the last node
		var lastNodeIndex = tiers.Last()[0];
		_nodeTypes[lastNodeIndex] = MapNode.MapNodeType.Boss;

		for ( var i = 0; i < _nodePositions.Count; i++ )
		{
			var node = new MapNode
			{
				Index = i,
				Type = _nodeTypes[i],
				Style =
				{
					Left = Length.Percent( _nodePositions[i].x ), Top = Length.Percent( _nodePositions[i].y )
				}
			};
			var i1 = i;
			node.AddEventListener( "onclick", () => Select( i1 ) );
			node.AddEventListener( "onmouseout", OnNodeUnhover );
			node.AddEventListener( "onmouseover", () => OnNodeHover( i1 ) );

			Map?.AddChild( node );
		}

		MapManager.MapGenerated = true;
		MapManager.Index = 0;
		UpdateNodeStates();

		// Generate connections
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
				var extra = _seededRandom.Next( 0, next.Count + 1 );
				var shuffled = next.OrderBy( _ => _seededRandom.Next() ).ToList();
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
				var hasOutgoing = _mapConnections.Any( c => c.From == from );
				if ( hasOutgoing )
				{
					continue;
				}

				var to = next[_seededRandom.Next( next.Count )];
				AddConnection( from, to );
			}
		}

		void AddConnection( int from, int to )
		{
			_mapConnections.Add( new MapConnection( from, to, null! ) );
		}
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
			return;

		Map.ChildrenOfType<Panel>()
			.Where( p => p.HasClass( "map-line" ) )
			.ToList()
			.ForEach( p => p.Delete() );

		foreach ( var conn in _mapConnections )
		{
			var fromNode = Nodes[conn.From];
			var toNode = Nodes[conn.To];

			var width = fromNode.Box.Rect.Width;
			var height = fromNode.Box.Rect.Height;
			var nodeSize = new Vector2( width, height );

			var from = GetPixelPosition( _nodePositions[conn.From] ) + nodeSize / 2;
			var to = GetPixelPosition( _nodePositions[conn.To] ) + nodeSize / 2;

			var dx = to.x - from.x;
			var dy = to.y - from.y;

			var distance = MathF.Sqrt( dx * dx + dy * dy );
			var angle = MathF.Atan2( dy, dx ) * (180 / MathF.PI);

			var linePanel = new Panel();
			linePanel.AddClass( "map-line" );
			linePanel.Style.Left = Length.Pixels( from.x );
			linePanel.Style.Top = Length.Pixels( from.y );
			linePanel.Style.Width = Length.Pixels( distance );
			linePanel.Style.Set( "transform-origin", "0 0" );
			linePanel.Style.Set( "transform", $"rotate({angle}deg)" );

			Map.AddChild( linePanel );
			conn.LinePanel = linePanel;
		}
	}

	private void Update()
	{
		UpdateNodeStyles();
		RenderLines();
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
			var isEnabled = i == CurrentNodeIndex || (IsCurrentNodeCompleted() && validNextNodes.Contains( i ));
			node.Enabled = isEnabled;
			node.SetClass( "disabled", !isEnabled );
			node.SetClass( "current", i == CurrentNodeIndex );
		}
	}

	private int _hoveredNode = -1;

	public void OnNodeHover( int index )
	{
		_hoveredNode = index;
		UpdateLineHighlights();
	}

	public void OnNodeUnhover()
	{
		_hoveredNode = -1;
		UpdateLineHighlights();
	}

	private void UpdateLineHighlights()
	{
		foreach ( var conn in _mapConnections )
		{
			conn.LinePanel.RemoveClass( "highlighted" );
		}

		if ( _hoveredNode == -1 )
		{
			return;
		}

		foreach ( var conn in _mapConnections )
		{
			if ( conn.From == _hoveredNode )
			{
				conn.LinePanel.AddClass( "highlighted" );
			}
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

		if ( !IsCurrentNodeCompleted() )
		{
			MapManager.Index = index;
			SaveManager.ActiveRunData.MapNodeIndex = index;

			UpdateNodeStates();
			RunNode( Nodes[index] );
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
		UpdateNodeStates();
		//RunNode( Nodes[index] );

		Log.Info( $"Selected node at index {index}" );
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
					if ( !MapManager.FloorBattles.TryGetValue( GameManager.Floor, out var battles ) )
					{
						Log.Error( $"No battles set on floor {GameManager.Floor}!" );
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
					if ( !MapManager.FloorEvents.TryGetValue( GameManager.Floor, out var events ) )
					{
						Log.Error( $"No events set on floor {GameManager.Floor}!" );
						return;
					}

					var randomEvent = Game.Random.FromList( events! );
					if ( randomEvent is not null )
					{
						_eventPanel ??= new EventPanel();
						_eventPanel.ShowEventById( randomEvent );

						Panel.AddChild( _eventPanel );
						node.Completed = true;
					}
					break;
				}
			default:
				throw new ArgumentOutOfRangeException( node.Type.ToString() );
		}
	}

	public void ShowShop()
	{
		_shopPanel ??= new ShopPanel();
		Panel.AddChild( _shopPanel );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Map, CurrentNodeIndex, GameManager?.Floor, Player.Local?.Money );
	}
}
