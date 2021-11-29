using missile.UI;
using Missile.Player;
using Sandbox;


namespace Missile
{
	public partial class Game : Sandbox.Game
	{
		public Game()
		{
			if ( IsServer )
			{
				// Create the HUD
				_ = new MissileGameHud();
			}
		}
		public override void ClientJoined( Client cl )
		{
			var pawn = new Missile.Player.HumanPlayer();
			pawn.Respawn();
			cl.Pawn = pawn;
			base.ClientJoined( cl );
		}

		public static Entity Swap( Entity prev )
		{
			var currentType = prev.GetType();
			Client cl = prev.Client;
			Sandbox.Player newEnt;

			if ( currentType == typeof( HumanPlayer ) )
			{
				newEnt = new MissilePlayer();
			}
			else
			{
				newEnt = new HumanPlayer();
			}


			newEnt.Respawn();
			cl.Pawn = newEnt;
			newEnt.Velocity = Vector3.Zero;
			newEnt.Position = prev.Position;

			prev.Delete();

			return newEnt;

		}

	}
}
