using Sandbox;


namespace Missile.Player
{
	public partial class MissileController : BasePlayerController
	{
		[Net] [Predicted] public float Thrust { get; set; } = 1f;

		[Net] [Predicted] public Vector3 ThrustVector { get; set; } = Vector3.One;

		[Net] [Predicted] public float roll { get; set; }

		[Net] [Predicted] public bool SpawnGracePeriodFinished { get; set; } = false;

		public MissileController()
		{
			ThrustVector = Vector3.Up * Game.MaxThrust;
			Thrust = Game.MaxThrust;
			DoGracePeriod();
		}

		private async void DoGracePeriod()
		{
			await GameTask.DelayRealtimeSeconds( 0.5f );
			SpawnGracePeriodFinished = true;
		}

		public override void Simulate()
		{
			Thrust = Thrust.Approach( 0, Time.Delta * 8f );

			if ( Input.Down( InputButton.Forward ) || Input.Down( InputButton.Run ) )
			{
				Thrust = Thrust.Approach( Game.MaxThrust, Time.Delta * 32f );
			}
			else if ( Input.Down( InputButton.Back ) || Input.Down( InputButton.Jump ) )
			{
				Thrust = Thrust.Approach( Thrust * 0.85f, Time.Delta * 35f );
			}

			if ( SpawnGracePeriodFinished )
				ThrustVector = Vector3.Lerp( ThrustVector, Input.Rotation.Forward.Normal * Thrust, 4f * Time.Delta );

			Velocity = ThrustVector + (Vector3.Down * 350f * Time.Delta);
			Position += Velocity;

			Rotation = Rotation.LookAt( Velocity, Vector3.Up ) * Rotation.FromRoll( roll );
			roll += (Velocity.Length * 35) * Time.Delta;

			EyeRot = Input.Rotation;
			WishVelocity = Velocity;
			GroundEntity = null;
			BaseVelocity = Vector3.Zero;

			base.Simulate();
		}

	}
}
