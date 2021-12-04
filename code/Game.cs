using Missile.UI;
using Sandbox;

namespace Missile
{
	public partial class Game : Sandbox.Game
	{
		public const string VERSION = "0.2.0";
		public Game()
		{
			if ( IsServer )
			{
				Global.PhysicsSubSteps = 2;
				DoPrecache();
				// Create the HUD
				_ = new Hud();
			}

			if ( IsClient )
			{
				PostProcess.Add( new StandardPostProcess() );
				pp = PostProcess.Get<StandardPostProcess>();
			}
		}

		public override void OnKilled( Entity pawn )
		{
			base.OnKilled( pawn );
		}

		public override void ClientJoined( Client cl )
		{
			//Decide which team the client will be on.
			var teamDifference = NumHumans - NumMissiles;
			if ( teamDifference < 0 )
			{
				JoinTeam( cl, Team.Human );
			}
			else if ( teamDifference > 0 )
			{
				JoinTeam( cl, Team.Missile );
			}
			else
			{
				JoinRandomTeam( cl );
			}

			base.ClientJoined( cl );
		}

		public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
		{
			if ( TeamMen.Contains( cl ) )
				TeamMen.Remove( cl );

			if ( TeamMissile.Contains( cl ) )
				TeamMissile.Remove( cl );

			cl.SetValue( "team", null );
			base.ClientDisconnect( cl, reason );
		}

	}
}
