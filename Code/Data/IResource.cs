namespace CardGame.Data;

public interface IResource
{
	public Id Id { get; set; }

	/// <summary>
	/// Called after this resource has been constructed.
	/// </summary>
	public void OnInit()
	{
		
	}
}
