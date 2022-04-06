using Sandbox.UI;

namespace Missile.UI
{
	public class HumanPlayerPanel : Panel
	{
		public HumanPlayerPanel()
		{
			AddChild( new Health() );
			AddChild( new Crosshair() );
		}
	}
}
