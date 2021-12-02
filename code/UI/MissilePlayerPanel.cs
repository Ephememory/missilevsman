using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Missile.Player;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;


namespace Missile.UI
{
	public class MissilePlayerPanel : Panel
	{
		public MissilePlayer missilePlayerPawn { get; private set; } //We can only hope to not lose this cached reference..
		private Panel launchingStatus;
		public MissilePlayerPanel( MissilePlayer player )
		{
			missilePlayerPawn = player;
			AddChild( new MissileLifeTimeBar( player ) );
			launchingStatus = Add.Panel( "launch-status-container" );
			launchingStatus.Add.Label( "LAUNCHING MISSILE", "launch-status" );
		}

		public override void Tick()
		{
			launchingStatus.SetClass( "active", !(missilePlayerPawn.Controller as MissileController).SpawnGracePeriodFinished );
			base.Tick();
		}
	}
}
