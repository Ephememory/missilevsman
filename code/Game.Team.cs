using System.Collections.Generic;
using Missile.UI;
using Missile.Player;
using Sandbox;


namespace Missile
{
	public partial class Game : Sandbox.Game
	{

		private List<Client> TeamMissile = new();
		private List<Client> TeamMen = new();

		public int NumHumans => TeamMen.Count;
		public int NumMissiles => TeamMissile.Count;

		public enum Team : int
		{
			Missile = 0,
			Human = 1
		}

		[ServerCmd( "missile_switch_team" )]
		public static void SwitchTeam()
		{
			var cl = ConsoleSystem.Caller;
			var prev = cl.Pawn;

			var currentType = prev.GetType();
			Sandbox.Player newEnt;

			if ( currentType == typeof( HumanPlayer ) )
			{
				newEnt = new MissilePlayer();
				cl.SetValue( "team", ((int)Team.Missile) );
			}
			else
			{
				newEnt = new HumanPlayer( cl );
				cl.SetValue( "team", ((int)Team.Human) );
			}

			cl.Pawn = newEnt;
			prev.Delete();
			newEnt.Respawn();
		}

		public void JoinTeam( Client client, Team whichTeam )
		{
			Sandbox.Player pawn = null;
			if ( whichTeam == Team.Human )
			{
				pawn = new HumanPlayer( client );
			}
			else
			{
				pawn = new MissilePlayer();
			}

			pawn.Respawn();
			client.Pawn = pawn;
			client.SetValue( "team", ((int)whichTeam) );
		}

		public void JoinRandomTeam( Client client )
		{
			var randomValue = Rand.Int( 0, 2 );
			Sandbox.Player pawn = null;
			if ( randomValue == 1 )
			{
				pawn = new MissilePlayer();
				client.SetValue( "team", ((int)Team.Missile) );
			}
			else
			{
				pawn = new HumanPlayer( client );
				client.SetValue( "team", ((int)Team.Human) );
			}

			pawn.Respawn();
			client.Pawn = pawn;
		}
	}

}
