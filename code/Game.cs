using Missile.UI;
using Sandbox;

namespace Missile
{
	public partial class Game : Sandbox.Game
	{
		public Game()
		{
			if ( IsServer )
			{
				Global.PhysicsSubSteps = 2;
				// Create the HUD
				_ = new MissileGameHud();
			}

			if ( IsClient )
			{
				PostProcess.Add( new StandardPostProcess() );
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
			else if ( teamDifference == 0 )
			{
				JoinRandomTeam( cl );
			}

			base.ClientJoined( cl );
		}

	}
}
