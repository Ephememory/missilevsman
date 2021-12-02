using System.Collections.Generic;
using Missile.UI;
using Missile.Player;
using Sandbox;


namespace Missile
{
	public partial class Game : Sandbox.Game
	{

		public void DoPrecache()
		{
			Precache.Add( "particles/explosion/barrel_explosion/explosion_barrel.vpcf" );
		}
	}
}
