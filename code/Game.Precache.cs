using Sandbox;

namespace Missile
{
	public partial class MvmGame : Sandbox.Game
	{

		public void DoPrecache()
		{
			Precache.Add( "particles/trail_missile.vpcf" );
			Precache.Add( "particles/explosion/barrel_explosion/explosion_barrel.vpcf" );
		}
	}
}
