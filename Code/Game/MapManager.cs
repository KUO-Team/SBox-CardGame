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

	public MapPanel Map => Scene.GetInstance<MapPanel>();

	// DON'T MAKE A PROPERTY
	public Dictionary<int, List<Id>> FloorBattles { get; set; } = new()
	{
		{
			3, [2]
		},
		{
			2, [2, 3, 4]
		},
		{
			1, [2, 3, 4, 5, 6]
		},
		{
			0, [3, 4, 5, 6]
		}
	};

	public Dictionary<int, List<Id>> FloorBosses { get; set; } = new()
	{
		{
			3, [3]
		},
		{
			2, [6]
		},
		{
			1, [2]
		},
		{
			0, [2]
		}
	};

	public Dictionary<int, List<Id>> FloorEvents { get; set; } = new()
	{
		{
			3, [1]
		},
		{
			2, [1, 2, 3]
		},
		{
			1, [1, 2, 3]
		},
		{
			0, [1, 2, 3]
		}
	};

	private static readonly Logger Log = new( "MapManager" );

	public int GetTierCount()
	{
		return 10;
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
				SaveManager.Instance.ActiveRunData.CompletedNodes.Clear();
				SaveManager.Instance.ActiveRunData.MapNodeIndex = 0;
				PlayerData.Save();
			}
		}

		Log.Info( $"New floor generated with seed: {Seed}" );
	}
}
