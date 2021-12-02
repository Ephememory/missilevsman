using Sandbox;

namespace Missile
{
	public partial class Game : Sandbox.Game
	{
		[ConVar.Replicated( "missile_max_thrust" )]
		public static float MaxThrust { get; set; } = 40f;

		[ConVar.Replicated( "missile_debug" )]
		public static bool Debug { get; set; } = false;

		[ConVar.Replicated( "missile_lifetime" )]
		public static int MaxLifeTime { get; set; } = 15;
	}
}
