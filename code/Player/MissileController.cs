using Sandbox;


namespace Missile.Player
{
	public partial class MissileController : BasePlayerController
	{

		public const float DefaultThrust = 20f;
		[Net] public float Thrust { get; private set; } = 1f;

		[Net] public Vector3 ThrustVector { get; private set; } = Vector3.One;

		[Net] public float Modifier { get; private set; } = 0;

		[Net] public float CurrentThrust => Thrust + Modifier;

		[Net] private float roll { get; set; }


		[ConVar.Replicated("missile_max_thrust")]
		public static float MaxThrust { get; set; } = 50f;


		public override void Simulate()
		{
			Thrust = Thrust.Approach( 0, Time.Delta * 8f );

			if ( Input.Down( InputButton.Forward ) || Input.Down( InputButton.Run ) )
			{
				Thrust = Thrust.Approach( MaxThrust, Time.Delta * 32f );
			}
			else if ( Input.Down( InputButton.Back ) || Input.Down(InputButton.Jump ) )
			{
				Thrust = Thrust.Approach( Thrust * 0.85f, Time.Delta * 35f );
			}

			ThrustVector = Vector3.Lerp( ThrustVector, Input.Rotation.Forward.Normal * Thrust , 4f * Time.Delta );
			Velocity = ThrustVector + (Vector3.Down * 350f * Time.Delta);
			Position += Velocity;

			Rotation = Rotation.LookAt( Velocity, Vector3.Up ) * Rotation.FromRoll( roll );
			roll += (Velocity.Length * 35) * Time.Delta;

			base.Simulate();
		}
	}
}
