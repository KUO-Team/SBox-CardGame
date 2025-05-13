using System;
using Sandbox.Diagnostics;

namespace Sandbox;

public static class LoggerExtensions
{
	public static void EditorLog( this Logger logger, object message, LogType type = LogType.Info )
	{
		if ( !Game.IsEditor )
		{
			return;
		}
		
		switch ( type )
		{
			case LogType.Info:
				logger.Info( message );
				break;
			case LogType.Warning:
				logger.Warning( message );
				break;
			case LogType.Error:
				logger.Error( message );
				break;
			case LogType.Trace:
				logger.Trace( message );
				break;
			default:
				throw new ArgumentOutOfRangeException( nameof( type ), type, null );
		}
	}

	public enum LogType
	{
		Info,
		Warning,
		Error,
		Trace
	}
}
