using Missile.Util;
using Missile.Camera;
using Sandbox;
using System;

namespace Missile.Player
{
	public partial class MissilePlayer : Sandbox.Player
	{
		static readonly Model model = Model.Load( "models/missile/missile.vmdl" );
		public static event Action<MissilePlayer> OnSpawned;
		private TraceResult sphereCast { get; set; }

		[Net]
		public float SphereCastRadius { get; private set; } = 5;

		[Net]
		public TimeSince TimeSinceLaunch { get; private set; } = 0;

		private TimeSince timeSinceDied;

		private float colorDesaturateAmount = 0f;

		private Color renderTint;

		private Particles trail;

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
			if ( Owner == Local.Client )
			{
				colorDesaturateAmount = 0.3f;

				var pp = PostProcess.Get<StandardPostProcess>();
				pp.Saturate.Enabled = true;
				pp.Saturate.Amount = colorDesaturateAmount;

				(Camera as MissileCamera).DoClientRespawn();
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
			WaterLevel.Clear();

			Controller = new MissileController();
			Animator = new MissileAnimator();
			Camera = new MissileCamera();

			EnableDrawing = true;
			EnableAllCollisions = false;
			SetModel( model );

			RenderColor = renderTint;
			CollisionGroup = CollisionGroup.Player;
			AddCollisionLayer( CollisionLayer.Player );
			MoveType = MoveType.MOVETYPE_WALK;
			EnableHitboxes = true;

			Game.Current?.MoveToSpawnpoint( this );
			Position += CollisionBounds.Maxs * 2f;
			ResetInterpolation();
			ClientRespawn( To.Single( this ) );
		}


		[ClientRpc]
		public void ClientRespawn()
		{
			//We only use this action to nofity the client-side HUD to change.
			OnSpawned?.Invoke( this );

			colorDesaturateAmount = 0.3f;

			var pp = PostProcess.Get<StandardPostProcess>();
			pp.Saturate.Enabled = true;
			pp.Saturate.Amount = colorDesaturateAmount;

			(Camera as MissileCamera).DoClientRespawn();
		}

		[Event.Frame]
		private void OnFrame()
		{
			if ( Game.Debug )
			{
				DebugOverlay.Sphere( Position + LocalRotation.Forward * 25f, SphereCastRadius, Color.Yellow );
			}

		}

		[Event.Tick.Server]
		private void OnTickServer()
		{
			if ( LifeState != LifeState.Alive ) return;
			if ( false == (Controller as MissileController).SpawnGracePeriodFinished ) return;
			if ( TimeSinceLaunch >= Game.MaxLifeTime )
			{
				Explode();
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
				if ( timeSinceDied > 3 && IsServer )
				{
					Respawn();
				}

				return;
			}

			if ( LifeState != LifeState.Alive ) return;

			var controller = GetActiveController();
			controller?.Simulate( cl, this, GetActiveAnimator() );

		}

		public override void FrameSimulate( Client cl )
		{
			if ( colorDesaturateAmount >= 100 ) return;
			var pp = PostProcess.Get<StandardPostProcess>();
			pp.Saturate.Enabled = true;
			pp.Saturate.Amount = colorDesaturateAmount;
			colorDesaturateAmount = colorDesaturateAmount.Approach( 100f, Time.Delta );
			base.FrameSimulate( cl );
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
			(Camera as MissileCamera).DoClientKilled();
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

			// DebugOverlay.Sphere( Position, explosion.Radius, Color.Red, true, 5 );

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
