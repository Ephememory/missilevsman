using Missile.Util;
using Missile.Camera;
using Sandbox;
using System;

namespace Missile.Player
{
	public partial class HumanPlayer : Sandbox.Player
	{
		static readonly Model model = Model.Load( "models/citizen/citizen.vmdl" );
		public static event Action<HumanPlayer> OnSpawned;
		public Clothing.Container Clothing = new();
		private TimeSince timeSinceDied = 0;

		public HumanPlayer()
		{
			Transmit = TransmitType.Always;
		}

		public HumanPlayer( Client cl ) : this()
		{
			// Load clothing from client data
			Clothing.LoadFromClient( cl );
		}

		//The rpc wont get called on the first spawn in
		public override void ClientSpawn()
		{
			if ( Owner == Local.Client )
			{
				OnSpawned?.Invoke( this );

				var pp = PostProcess.Get<StandardPostProcess>();
				pp.Saturate.Enabled = false;
				pp.Saturate.Amount = 0f;
			}
			base.ClientSpawn();
		}

		public override void Respawn()
		{
			base.Respawn();
			SetModel( model );
			Controller = new WalkController();
			Animator = new StandardPlayerAnimator();
			Camera = new ThirdPersonCamera();

			Clothing.DressEntity( this );

			EnableTouch = true;
			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;
			Transmit = TransmitType.Always;
			ClientRespawn( To.Single( Client ) );
		}

		[ClientRpc]
		public void ClientRespawn()
		{
			Log.Info( Client );
			OnSpawned?.Invoke( this );

			var pp = PostProcess.Get<StandardPostProcess>();
			pp.Saturate.Enabled = false;
			pp.Saturate.Amount = 0f;

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

	}

}
