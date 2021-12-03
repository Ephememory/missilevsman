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
		private Panel tutorial;
		private float tutorialFadeout = 1;
		public MissilePlayerPanel( MissilePlayer player )
		{
			missilePlayerPawn = player;
			AddChild( new MissileLifeTimeBar( player ) );

			tutorial = Add.Panel( "missile-tutorial" );
			tutorial.Add.Label( "FORWARD or RUN to accelerate\nBACK or JUMP to decelerate\nMOUSE to steer" );

			launchingStatus = Add.Panel( "launch-status-container" );
			launchingStatus.Add.Label( "LAUNCHING MISSILE", "launch-status" );
		}

		public override void Tick()
		{
			launchingStatus.SetClass( "active", !(missilePlayerPawn.Controller as MissileController).SpawnGracePeriodFinished );
			tutorialFadeout = MathX.Approach( tutorialFadeout, 0, Time.Delta / 6 );
			tutorial.Style.Opacity = tutorialFadeout;
			tutorial.Style.Dirty();
			base.Tick();
		}
	}
}
