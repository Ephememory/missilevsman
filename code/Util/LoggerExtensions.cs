using Sandbox;

namespace Missile.Util
{
	public static class LoggerExtensions
	{
		public static void SidedInfo( this Logger logger, object message )
		{
			var side = Host.IsServer ? "[SERVER]" : "[client]-";
			logger.Info( $"{side}: {message}" );
		}
	}
}
