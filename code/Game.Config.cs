using Sandbox;

namespace Missile
{
	public partial class MvmGame : Sandbox.Game
	{
		[ConVar.Replicated( "missile_max_thrust" )]
		public static float MaxThrust { get; set; } = 40f;

		[ConVar.Replicated( "missile_debug" )]
		public static bool Debug { get; set; } = false;

		[ConVar.Replicated( "missile_lifetime" )]
		public static int MaxLifeTime { get; set; } = 15;

		[ConVar.Replicated( "missile_respawn_timer" )]
		public static int RespawnTimer { get; set; } = 3;
	}
}
