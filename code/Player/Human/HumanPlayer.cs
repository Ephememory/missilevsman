using Missile.Util;
using Missile.Camera;
using Sandbox;
using System;

namespace Missile.Player
{
	public partial class HumanPlayer : Sandbox.Player
	{
		public static event Action<HumanPlayer> OnSpawned;
		public Clothing.Container Clothing = new();
		private TimeSince timeSinceDied = 0;

		public HumanPlayer()
		{

		}

		public HumanPlayer( Client cl ) : this()
		{
			// Load clothing from client data
			Clothing.LoadFromClient( cl );
		}

		public override void Spawn()
		{
			// RootPanel.AddChild<HumanPlayerPanel>();
			base.Spawn();
		}

		public override void Respawn()
		{
			base.Respawn();
			SetModel( "models/citizen/citizen.vmdl" );
			Controller = new WalkController();
			Animator = new StandardPlayerAnimator();
			Camera = new ThirdPersonCamera();

			Clothing.DressEntity( this );

			EnableTouch = true;
			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;
		}

		public override void ClientSpawn()
		{
			// Local.Hud.CreateEvent( "humanspawned", this );
			OnSpawned?.Invoke( this );
			base.ClientSpawn();
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

			var controller = GetActiveController();
			controller?.Simulate( cl, this, GetActiveAnimator() );

		}

		public override void OnKilled()
		{
			timeSinceDied = 0;
			EnableDrawing = false;
			EnableAllCollisions = false;
			base.OnKilled();
		}
		public override void Touch( Entity other )
		{
			Log.Info( other );
			base.Touch( other );
		}

		public override void StartTouch( Entity other )
		{
			Log.Info( other );
			base.StartTouch( other );
		}

		public override void EndTouch( Entity other )
		{
			Log.Info( other );
			base.EndTouch( other );
		}
	}

}
