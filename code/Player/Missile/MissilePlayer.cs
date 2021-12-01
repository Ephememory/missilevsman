using Missile.Util;
using Missile.Camera;
using Sandbox;
using System;

namespace Missile.Player
{
	public partial class MissilePlayer : Sandbox.Player
	{
		public static event Action<MissilePlayer> OnSpawned;
		private TraceResult hitBoxTraceResult { get; set; }

		[Net]
		public int LifeTime { get; private set; } = 0;

		TimeSince timeSinceDied;

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
			LifeTime = 0;
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

		}

		[Event.Frame]
		private void OnFrame()
		{
			if ( Game.Debug )
			{
				DebugOverlay.Sphere( Position + LocalRotation.Forward * 25f, 4.5f, Color.Yellow );
			}

		}

		[Event.Tick.Server]
		private void OnTickServer()
		{
			if ( LifeState != LifeState.Alive ) return;

			if ( LifeTime >= Game.MaxLifeTime )
			{
				Explode();
			}
			else
			{
				LifeTime++;
			}

		}

		[Event.Physics.PreStep]
		private void OnPhysicsPreStep()
		{
			if ( LifeState != LifeState.Alive ) return;
			if ( IsServer )
			{
				hitBoxTraceResult = Trace.Sphere( 4.5f, Position, Position + LocalRotation.Forward * 25f ).Ignore( this ).Run();

				if ( hitBoxTraceResult.Hit )
				{
					Log.SidedInfo( hitBoxTraceResult.Entity );
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

		public override void OnKilled()
		{
			EnableDrawing = false;
			timeSinceDied = 0;
			base.OnKilled();
		}

		private void Explode()
		{
			if ( LifeState != LifeState.Alive ) return;
			new ExplosionEntity
			{
				Position = Position,
				Radius = 50f,
				Damage = 1000,

			}.Explode( this );

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
