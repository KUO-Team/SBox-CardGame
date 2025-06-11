using System;
using Sandbox.UI;
using CardGame.Data;

namespace CardGame.UI.Map;

public partial class MapPanel
{
	public List<MapConnection> MapConnections { get; private set; } = [];
	public List<MapNode> Nodes => Map?.ChildrenOfType<MapNode>().OrderBy( n => n.Index ).ToList() ?? [];
	
	private readonly List<MapNode.MapNodeType> _nodeTypes = [];
	private readonly List<Vector2> _nodePositions = [];

	public void GenerateMapLayout( int? seed = null )
	{
		if ( !Map.IsValid() || !GameManager.IsValid() || !MapManager.IsValid() )
		{
			Log.Warning( "Unable to generate map; invalid state." );
			return;
		}

		Log.Info( seed.HasValue ? $"Generating map layout with seed {seed.Value}" : "Generating random map layout" );

		_nodePositions.Clear();
		_nodeTypes.Clear();
		MapConnections.Clear();

		_seededRandom = seed.HasValue ? new Random( seed.Value ) : new Random();
		var totalTiers = MapManager.GetTierCount();
		var maxNodesPerTier = MapManager.GetMaxNodesPerTier();

		var tiers = GenerateTiers( totalTiers, maxNodesPerTier );
		InjectShopNode( tiers );
		InjectEventNodes( tiers );
		InjectEliteNode( tiers );

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

	private void InjectEliteNode( List<List<int>> tiers )
	{
		if ( !GameManager.IsValid() || !MapManager.IsValid() )
		{
			return;
		}

		// Check if elites exist for this floor
		if ( !MapManager.FloorElites.TryGetValue( GameManager.Floor, out var elites ) || elites.Count == 0 || tiers.Count <= 3 )
		{
			return;
		}

		// Skip tier 1 entirely — start from tier 2, but also exclude the last tier (boss)
		var eligibleTiers = Enumerable.Range( 2, tiers.Count - 3 ).ToList();
		if ( eligibleTiers.Count == 0 )
		{
			return;
		}

		var randomTier = eligibleTiers[_seededRandom.Next( eligibleTiers.Count )];
		var candidates = tiers[randomTier].Where( i => _nodeTypes[i] == MapNode.MapNodeType.Battle ).ToList();

		if ( candidates.Count == 0 )
		{
			return;
		}

		var eliteIndex = candidates[_seededRandom.Next( candidates.Count )];
		_nodeTypes[eliteIndex] = MapNode.MapNodeType.Elite;
	}

	private void InjectEventNodes( List<List<int>> tiers )
	{
		if ( !GameManager.IsValid() || !MapManager.IsValid() )
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
		if ( !GameManager.IsValid() || !MapManager.IsValid() )
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
			case MapNode.MapNodeType.Elite:
				if ( MapManager.FloorElites.TryGetValue( GameManager.Floor, out var elites ) && elites.Count > 0 )
				{
					node.Battle = Game.Random.FromList( elites! );
				}
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
		MapConnections.Clear();

		for ( var i = 0; i < tiers.Count - 1; i++ )
		{
			var currentTier = tiers[i];
			var nextTier = tiers[i + 1];

			// Sort by Y position for consistent connections
			var sortedCurrent = currentTier.OrderBy( n => _nodePositions[n].y ).ToList();
			var sortedNext = nextTier.OrderBy( n => _nodePositions[n].y ).ToList();

			if ( currentTier.Count == 1 )
			{
				// Single node: connect to 2-3 nodes for choice
				var connectTo = Math.Min( 3, nextTier.Count );
				if ( nextTier.Count <= 3 )
				{
					// Connect to all if small tier
					foreach ( var node in nextTier )
					{
						AddConnection( currentTier[0], node );
					}
				}
				else
				{
					// Spread connections across tiers
					AddConnection( currentTier[0], nextTier[0] );
					AddConnection( currentTier[0], nextTier[^1] );
					if ( connectTo > 2 )
					{
						AddConnection( currentTier[0], nextTier[nextTier.Count / 2] );
					}
				}
			}
			else if ( nextTier.Count == 1 )
			{
				// Multiple to single: all must connect (prevents skipping)
				foreach ( var node in currentTier )
				{
					AddConnection( node, nextTier[0] );
				}
			}
			else
			{
				// Multi to multi: create overlapping paths
				for ( var j = 0; j < sortedCurrent.Count; j++ )
				{
					var fromNode = sortedCurrent[j];

					// Map to corresponding position in next tier
					var ratio = sortedCurrent.Count == 1 ? 0.5f : (float)j / (sortedCurrent.Count - 1);
					var primaryTarget = (int)Math.Round( ratio * (sortedNext.Count - 1) );

					AddConnection( fromNode, sortedNext[primaryTarget] );

					// Add one additional connection for choice (edge nodes get priority)
					var isEdge = j == 0 || j == sortedCurrent.Count - 1;
					var addExtra = isEdge || _seededRandom.NextDouble() < 0.4;

					if ( !addExtra || sortedNext.Count <= 1 )
					{
						continue;
					}
					var offset = j < sortedCurrent.Count / 2 ? -1 : 1;
					var extraTarget = Math.Clamp( primaryTarget + offset, 0, sortedNext.Count - 1 );

					if ( extraTarget != primaryTarget )
					{
						AddConnection( fromNode, sortedNext[extraTarget] );
					}
				}
			}

			// Ensure every node in next tier is reachable
			var reachableNext = MapConnections
				.Where( c => currentTier.Contains( c.From ) )
				.Select( c => c.To ).ToHashSet();

			foreach ( var unreachable in nextTier.Except( reachableNext ) )
			{
				var closest = currentTier.OrderBy( n => Math.Abs( _nodePositions[n].y - _nodePositions[unreachable].y ) ).First();
				AddConnection( closest, unreachable );
			}
		}
	}

	private void AddConnection( int from, int to )
	{
		MapConnections.Add( new MapConnection( from, to, null! ) );
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
}
