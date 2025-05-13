namespace CardGame.Data;

public class Settings
{
	public static Settings Data
	{
		get
		{
			return FileSystem.Data.ReadJson( FileName, new Settings() );
		}
	}

	public const string FileName = "Settings.json";

	public float MasterVolume { get; set; } = 1f;
	
	public float MusicVolume { get; set; } = 1f;
	
	public float GameVolume { get; set; } = 1f;
	
	public float UIVolume { get; set; } = 1f;
	
	public void Save()
	{
		FileSystem.Data.WriteJson( FileName, this );
	}
}
