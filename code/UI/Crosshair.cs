using Sandbox;
using Sandbox.UI;

namespace Missile.UI
{
	[UseTemplate]
	public class Crosshair : Panel
	{
		public Crosshair()
		{
			AddClass( "crosshair" );
			var inner = new Panel();
			inner.AddClass( "inner" );
			AddChild( inner );
		}
	}
}
