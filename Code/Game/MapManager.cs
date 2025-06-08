using Sandbox.Diagnostics;
using CardGame.Data;
using CardGame.UI.Map;

namespace CardGame;

public sealed class MapManager : Singleton<MapManager>
{
	[Property]
	public int Seed { get; set; }

	/// <summary>
	/// The position we were last on the map.
	/// </summary>
	[Property, ReadOnly]
	public int Index { get; set; }

	[Property, ReadOnly]
	public bool MapGenerated { get; set; }

	[Property]
	public int NodesCompleted { get; set; }

	[Property]
	public int EnemyLevel { get; set; } = 1;
	
	public MapPanel? Map => Scene.GetInstance<MapPanel>();

	public Dictionary<int, List<Id>> FloorBattles { get; set; } = new()
	{
		{
			3, [3]
		},
		{
			2, [3, 4, 5, 6]
		},
		{
			1, [3, 4, 5, 6]
		},
		{
			0, [3, 4, 5, 6]
		}
	};
	
	public Dictionary<int, List<Id>> FloorElites { get; set; } = new()
	{
		{
			3, [2]
		},
		{
			2, [2]
		},
		{
			1, [2, 7]
		},
		{
			0, [2, 7]
		}
	};

	public Dictionary<int, List<Id>> FloorBosses { get; set; } = new()
	{
		{
			3, [4]
		},
		{
			2, [7]
		},
		{
			1, [7]
		},
		{
			0, [7]
		}
	};

	public Dictionary<int, List<Id>> FloorEvents { get; set; } = new()
	{
		{
			3, [1, 2]
		},
		{
			2, [1, 2, 3, 4]
		},
		{
			1, [1, 2, 3, 4]
		},
		{
			0, [1, 2, 3, 4]
		}
	};

	private static readonly Logger Log = new( "MapManager" );

	public int GetTierCount()
	{
		var floor = GameManager.Instance?.Floor ?? 0;
		return floor switch
		{
			3 or 2 => 10,
			1 or 0 => 12,
			_ => 10
		};
	}

	public int GetMaxNodesPerTier()
	{
		var floor = GameManager.Instance?.Floor ?? 0;
		return floor switch
		{
			3 or 2 => 3,
			1 or 0 => 4,
			_ => 3
		};
	}

	public void GenerateNewFloor()
	{
		if ( !Map.IsValid() )
		{
			Log.Warning( $"No map panel found; unable to generate floor!" );
			return;
		}

		GameManager.Instance?.NextFloor();
		Map.DeleteLayout();
		Map.GenerateMapLayout( Seed );
		Index = 0;

		if ( SaveManager.Instance.IsValid() )
		{
			if ( SaveManager.Instance.ActiveRunData is not null )
			{
				NodesCompleted = 0;
				SaveManager.Instance.ActiveRunData.CompletedNodes.Clear();
				SaveManager.Instance.ActiveRunData.MapNodeIndex = 0;
				PlayerData.Save();
			}
		}

		Log.Info( $"New floor generated with seed: {Seed}" );
	}
}
