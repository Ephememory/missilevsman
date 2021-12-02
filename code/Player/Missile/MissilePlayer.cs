using Missile.Util;
using Missile.Camera;
using Sandbox;
using System;

namespace Missile.Player
{
	public partial class MissilePlayer : Sandbox.Player
	{
		public static event Action<MissilePlayer> OnSpawned;
		private TraceResult sphereCast { get; set; }

		[Net]
		public float SphereCastRadius { get; private set; } = 5;

		[Net]
		public TimeSince TimeSinceLaunch { get; private set; } = 0;

		private TimeSince timeSinceDied;

		private float colorDesaturateAmount = 0f;

		public MissilePlayer()
		{
		}

		public override void Spawn()
		{
			base.Spawn();
		}

		public override void ClientSpawn()
		{
			//We only use this action to nofity the client-side HUD to change.
			OnSpawned?.Invoke( this );
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
			SetModel( "models/missile/missile.vmdl" );

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

			Log.SidedInfo( TimeSinceLaunch );
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
					Log.SidedInfo( sphereCast.Entity );
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

			DebugOverlay.Sphere( Position, explosion.Radius, Color.Red, true, 5 );

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
