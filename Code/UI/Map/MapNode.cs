using System;
using Sandbox.UI;
using CardGame.Data;

namespace CardGame.UI.Map;

public class MapNode : Panel
{
	[Parameter]
	public int Index { get; set; }

	[Parameter]
	public bool Completed { get; set; }
	
	[Parameter]
	public bool Enabled { get; set; } = true;
	
	[Parameter]
	public MapNodeType Type { get; set; } = MapNodeType.Battle;

	[Parameter]
	public Id? Battle { get; set; }
	
	[Parameter]
	public Id? Event { get; set; }

	private Label? _label;
	private BattleInfoPanel? _battleInfoPanel;

	private static GameManager? GameManager => GameManager.Instance;
	private static MapManager? MapManager => MapManager.Instance;

	public MapNode()
	{
		AddClass( "node" );
	}

	protected override void OnParametersSet()
	{
		Init();
		base.OnParametersSet();
	}

	private void Init()
	{
		DeleteChildren( true );

		_label = new Label
		{
			Text = Type switch
			{
				MapNodeType.Start => "X",
				MapNodeType.Shop => "S",
				MapNodeType.Event => "?",
				MapNodeType.Battle => "B",
				MapNodeType.Elite => "E",
				MapNodeType.Boss => "!",
				_ => "?"
			}
		};

		AddChild( _label );
	}

	public void Run( MapPanel map )
	{
		if ( !MapManager.IsValid() || !GameManager.IsValid() )
		{
			return;
		}

		switch ( Type )
		{
			case MapNodeType.Battle:
				{
					if ( Battle is null )
					{
						Log.Error( $"No battle set on node {this}!" );
						return;
					}

					var battle = BattleDataList.GetById( Battle );
					if ( battle is null )
					{
						return;
					}

					_battleInfoPanel?.Delete();
					_battleInfoPanel = new BattleInfoPanel
					{
						Battle = battle
					};

					map.Panel.AddChild( _battleInfoPanel );
					break;
				}
			case MapNode.MapNodeType.Elite:
				{
					if ( Battle is null )
					{
						Log.Error( $"No elite battle set on node {this}!" );
						return;
					}

					var battle = BattleDataList.GetById( Battle );
					if ( battle is null )
					{
						return;
					}

					_battleInfoPanel?.Delete();
					_battleInfoPanel = new BattleInfoPanel
					{
						Battle = battle
					};

					map.Panel.AddChild( _battleInfoPanel );
					break;
				}
			case MapNode.MapNodeType.Shop:
				{
					map.ShowShop();
					Completed = true;
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

						map.Panel.AddChild( _battleInfoPanel );
					}
					break;
				}
			case MapNode.MapNodeType.Event:
				{
					if ( Event is null )
					{
						Log.Error( $"No event set on node {this}!" );
						return;
					}

					if ( !map.EventPanel.IsValid() )
					{
						Log.Error( $"Unable to show event; no event panel set!" );
						return;
					}
					map.EventPanel.ShowEventById( Event );
					Completed = true;
					break;
				}
			default:
				throw new ArgumentOutOfRangeException( Type.ToString() );
		}
	}

	protected override void OnClick( MousePanelEvent e )
	{
		if ( !Enabled )
		{
			return;
		}
		
		base.OnClick( e );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Type, Battle );
	}

	public enum MapNodeType
	{
		Start,
		Shop,
		Event,
		Battle,
		Elite,
		Boss,
	}
}
