using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace missile.Util
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
