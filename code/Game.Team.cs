using Missile.Player;
using Sandbox;
using System.Collections.Generic;

namespace Missile
{
	public partial class MvmGame : Sandbox.Game
	{
		public enum Team : int
		{
			Missile = 0,
			Human = 1
		}
		
		private static List<Client> TeamMissile = new();
		private static List<Client> TeamMen = new();

		public int NumHumans => TeamMen.Count;
		public int NumMissiles => TeamMissile.Count;

		[ServerCmd( "missile_switch_team" )]
		public static void SwitchTeam()
		{
			var cl = ConsoleSystem.Caller;
			var prev = cl.Pawn;

			var currentType = prev.GetType();
			Sandbox.Player newEnt;
			if ( currentType == typeof( HumanPlayer ) )
			{
				TeamMen.Remove( cl );
				newEnt = new MissilePlayer( ColorFromPlayerId( cl.PlayerId ) );
				cl.SetValue( "team", ((int)Team.Missile) );
				TeamMissile.Add( cl );
			}
			else
			{
				TeamMissile.Remove( cl );
				newEnt = new HumanPlayer( cl );
				cl.SetValue( "team", ((int)Team.Human) );
				TeamMen.Add( cl );
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
				TeamMen.Add( client );
				Log.Info( $"{client.Name} joining team Human" );
			}
			else
			{
				pawn = new MissilePlayer( ColorFromPlayerId( client.PlayerId ) );
				TeamMissile.Add( client );
				Log.Info( $"{client.Name} joining team Missile" );
			}

			client.Pawn = pawn;
			pawn.Respawn();
			client.SetValue( "team", ((int)whichTeam) );
		}

		public void JoinRandomTeam( Client client )
		{
			var randomValue = Rand.Int( 0, 2 );
			Sandbox.Player pawn = null;
			if ( randomValue == 1 )
			{
				pawn = new MissilePlayer( ColorFromPlayerId( client.PlayerId ) );
				client.SetValue( "team", ((int)Team.Missile) );
				Log.Info( $"{client.Name} joining team Missile by RANDOM" );
			}
			else
			{
				pawn = new HumanPlayer( client );
				client.SetValue( "team", ((int)Team.Human) );
				Log.Info( $"{client.Name} joining team Human by RANDOM" );
			}

			client.Pawn = pawn;
			pawn.Respawn();
		}

		public static Color ColorFromPlayerId( long id )
		{
			if ( id <= 0 ) return Color.Random;
			if ( id == 76561197998255119 ) return Color.FromBytes( 212, 175, 55 );

			//eh, it KINDA works.
			byte r = (byte)(id >> 16);
			byte g = (byte)((id / 2) >> 2);
			byte b = (byte)((id / 4) >> 2);

			return Color.FromBytes( r, g, b );
		}
	}

}
