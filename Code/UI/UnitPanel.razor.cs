using CardGame.Data;
using Sandbox.UI;
using CardGame.Units;

namespace CardGame.UI;

public partial class UnitPanel
{
	[Property]
	public BattleUnit? Unit { get; set; }

	private UnitInfoPanel? _panel;

	private Battle.BattleUnit CreateById( Id id )
	{
		return new Battle.BattleUnit
		{
			Id = Unit?.Data?.Id ?? CardGame.Data.Id.Invalid
		};
	}
	
	protected override void OnMouseDown( MousePanelEvent e )
	{
		var hud = Hud.Instance;
		if ( !hud.IsValid() )
		{
			return;
		}

		if ( Unit?.Data is null )
		{
			return;
		}

		if ( !Unit.HealthComponent.IsValid() || Unit.HealthComponent.IsDead )
		{
			return;
		}
		
		if ( !_panel.IsValid() )
		{
			_panel ??= new UnitInfoPanel
			{
				BattleUnit = CreateById( Unit.Data.Id ),
			};
			hud.AddElement( _panel );
		}
		else
		{
			if ( _panel.HasClass( "hidden" ) )
			{
				_panel.Show();
			}
		}
		base.OnMouseDown( e );
	}
}
