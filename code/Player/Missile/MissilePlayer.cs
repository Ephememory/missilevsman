using Missile.Camera;
using Sandbox;
using Missile.UI;

namespace Missile.Player
{
	public partial class MissilePlayer : Sandbox.Player
	{
		static readonly Model model = Model.Load( "models/missile/missile.vmdl" );

		[Net]
		public float SphereCastRadius { get; set; } = 5;

		[Net]
		public TimeSince TimeSinceLaunch { get; set; } = 0;

		private TraceResult sphereCast { get; set; }

		private TimeSince timeSinceDied;

		private Color renderTint;
		private Particles trail;
		private MissilePlayerPanel hudPanel;

		public MissilePlayer()
		{
			Transmit = TransmitType.Always;
		}

		public MissilePlayer( Color color ) : base()
		{
			renderTint = color;
		}

		protected override void OnDestroy()
		{
			hudPanel?.Delete();
			trail?.Destroy();
			base.OnDestroy();
		}

		public override void Spawn()
		{
			trail = Particles.Create( "particles/trail_missile.vpcf", this, "nozzle", true );
			base.Spawn();
		}

		//Rpc fails to get called on the initial spawn in, even with Transmit.Always?
		public override void ClientSpawn()
		{
			if ( Local.Pawn == this )
			{
				hudPanel = new MissilePlayerPanel();
				Local.Hud.AddChild( hudPanel );
				ClientRespawn();
			}

			base.ClientSpawn();
		}

		public override void Respawn()
		{
			Host.AssertServer();

			TimeSinceLaunch = 0;
			LifeState = LifeState.Alive;
			Health = 100;
			Velocity = Vector3.Zero;
			WaterLevel = 0;

			Controller = new MissileController();
			Animator = new MissileAnimator();
			CameraMode = new MissileCamera();

			EnableDrawing = true;
			EnableAllCollisions = false;
			Model = model;

			RenderColor = renderTint;
			CollisionGroup = CollisionGroup.Player;
			AddCollisionLayer( CollisionLayer.Player );
			EnableHitboxes = true;

			MvmGame.Current?.MoveToSpawnpoint( this );
			Position += CollisionBounds.Maxs * 2f;
			ResetInterpolation();
			ClientRespawn( To.Single( this ) );
		}

		[ClientRpc]
		public void ClientRespawn()
		{
			(MvmGame.Current as Missile.MvmGame).PPSetSaturation( 0.2f );
			(CameraMode as MissileCamera).DoClientRespawn(); //could maybe do custom Events?
			hudPanel?.LifeTimeBar?.Reset();
			Event.Run( "missile_respawn" );
		}

		[Event.Frame]
		private void OnFrame()
		{
			if ( MvmGame.Debug )
			{
				DebugOverlay.Sphere( Position + LocalRotation.Forward * 25f, SphereCastRadius, Color.Yellow );
			}
		}

		[Event.Physics.PreStep]
		private void OnPhysicsPreStep()
		{
			if ( LifeState != LifeState.Alive ) return;

			if ( IsServer )
			{
				if ( false == (Controller as MissileController).SpawnGracePeriodFinished ) return;
				sphereCast = Trace.Sphere( SphereCastRadius, Position, Position + LocalRotation.Forward * 25f ).Ignore( this ).Run();

				if ( sphereCast.Hit )
				{
					Explode();
				}
			}
		}

		public override void Simulate( Client cl )
		{
			if ( LifeState == LifeState.Dead )
			{
				if ( timeSinceDied > MvmGame.RespawnTimer && IsServer )
				{
					Respawn();
				}

				return;
			}

			if ( LifeState != LifeState.Alive ) return;

			var controller = GetActiveController();
			controller?.Simulate( cl, this, GetActiveAnimator() );

			if ( false == (Controller as MissileController).SpawnGracePeriodFinished ) return;
			if ( TimeSinceLaunch >= MvmGame.MaxLifeTime )
			{
				Explode();
			}
		}

		public override void OnKilled()
		{
			EnableDrawing = false;
			timeSinceDied = 0;

			ClientOnKilled( To.Single( this ) );
			base.OnKilled();
		}

		[ClientRpc]
		public void ClientOnKilled()
		{
			(CameraMode as MissileCamera).DoClientKilled();
		}

		private void Explode()
		{
			if ( LifeState != LifeState.Alive ) return;
			var explosion = new ExplosionEntity
			{
				Position = Position,
				Radius = 150f,
				Damage = 1000,

			};
			explosion.Explode( this );
			ClientExplode();

			if ( IsServer )
			{
				OnKilled();
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
