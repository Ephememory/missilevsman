using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Missile.Util
{
	public static class LoggerExtensions
	{
		public static void SidedInfo( this Logger logger, object message )
		{
			if ( !Game.Debug ) return;
			var side = Host.IsServer ? "[SERVER]" : "[client]-";
			logger.Info( $"{side}: {message}" );
		}
	}
}
