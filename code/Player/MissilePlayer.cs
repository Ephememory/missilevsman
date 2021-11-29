using missile.Util;
using Missile.Camera;
using Sandbox;

namespace Missile.Player
{
	public partial class MissilePlayer : Sandbox.Player
	{
		private TraceResult hitBoxTraceResult { get; set; }

		[ConVar.Replicated( "missile_debug" )]
		public static bool Debug { get; set; } = false;

		public override void Spawn()
		{
			base.Spawn();
		}

		public override void Respawn()
		{
			Host.AssertServer();

			LifeState = LifeState.Alive;
			Health = 100;
			Velocity = Vector3.Zero;
			WaterLevel.Clear();

			Controller = new MissileController();
			Animator = new MissileAnimator();
			Camera = new CustomThirdPersonCamera();

			EnableAllCollisions = false;
			SetModel( "models/missile/missile.vmdl" );

			CollisionGroup = CollisionGroup.Player;
			AddCollisionLayer( CollisionLayer.Player );
			MoveType = MoveType.MOVETYPE_WALK;
			EnableHitboxes = true;

			Game.Current?.MoveToSpawnpoint( this );
			ResetInterpolation();

			Position += Vector3.Up * 155;

		}

		[Event.Frame]
		private void OnFrame()
		{
			if ( Debug )
			{
				DebugOverlay.Sphere( Position + LocalRotation.Forward * 25f, 4f, Color.Yellow );
			}

		}

		public override void Simulate( Client cl )
		{
			if ( IsServer )
			{

				hitBoxTraceResult = Trace.Sphere( 4f, Position, Position + LocalRotation.Forward * 25f ).Ignore( this ).Run();

				if ( IsServer && Input.Pressed( InputButton.Use ) )
				{
					Missile.Game.Swap( this );
				}

				if ( hitBoxTraceResult.Hit )
				{
					Log.SidedInfo( hitBoxTraceResult.Entity );
					Explode();
				}

			}

			var controller = GetActiveController();
			controller?.Simulate( cl, this, GetActiveAnimator() );
		}

		private void Explode()
		{
			new ExplosionEntity
			{
				Position = Position,
				Radius = 50f,
				Damage = 1000,

			}.Explode( this );

			// ClientExplode();

			if ( IsServer )
			{
				Respawn();
			}
		}


		[ClientRpc]
		public void ClientExplode()
		{
			new ExplosionEntity
			{
				Position = Position,
				Radius = 50f,
				Damage = 1000,

			}.Explode( this );
		}


	}
}
