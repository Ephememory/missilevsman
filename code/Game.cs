using Missile.UI;
using Sandbox;

namespace Missile
{
	public partial class MvmGame : Sandbox.Game
	{
		public const string VERSION = "0.2.1";
		public static MvmGame Instance => (Sandbox.Game.Current as MvmGame);

		public MvmGame()
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

		public override void DoPlayerSuicide( Client cl )
		{
			if ( cl.Pawn == null ) return;

			cl.Pawn.TakeDamage( DamageInfo.Generic( 1000 ).WithAttacker( cl.Pawn ) );
		}

		public override void ClientJoined( Client cl )
		{
			//Decide which team the client will be on.
			Log.Info( $"NumHumans:{NumHumans} | NumMissiles:{NumMissiles}" );
			if ( NumMissiles < NumHumans )
			{
				JoinTeam( cl, Team.Missile );
			}
			else if ( NumMissiles > NumHumans )
			{
				JoinTeam( cl, Team.Human );
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
