using System;
using Sandbox.UI;
using CardGame.Data;

namespace CardGame.UI;

public partial class RunPanel
{
	[Parameter]
	public RunData? Data { get; set; }
	
	public void LoadRun()
	{
		if ( Data is not null )
		{
			SaveManager.Instance?.Load( Data );
		}
	}
	
	public void DeleteData()
	{
		if ( Data is not null )
		{
			WarningPanel? warning = null;

			warning = WarningPanel.Create( "Delete Run Data", "This action will delete the provided run data. Are you sure?", [
				new Button( "Yes", "", () =>
				{
					SaveManager.Instance?.Delete( Data.Index );
					warning?.Delete();
				} ),
				new Button( "No", "", () =>
				{
					warning?.Delete();
				} )
			] );
		}
	}
	
	protected override int BuildHash()
	{
		return HashCode.Combine( Data, Data?.Date );
	}
}
