using Sandbox;
using Missile.UI;

namespace Missile.Player
{
	public partial class HumanPlayer : Sandbox.Player
	{
		static readonly Model model = Model.Load( "models/citizen/citizen.vmdl" );
		public Clothing.Container Clothing = new();
		private TimeSince timeSinceDied = 0;
		private HumanPlayerPanel hudPanel;

		public HumanPlayer()
		{
			Transmit = TransmitType.Always;
			Inventory = new Inventory( this );
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
				hudPanel = new HumanPlayerPanel();
				Local.Hud.AddChild( hudPanel );
				MvmGame.Instance.PPClearSaturation();
			}
			base.ClientSpawn();
		}

		public override void Respawn()
		{
			base.Respawn();
			Model = model;
			Controller = new WalkController();
			Animator = new StandardPlayerAnimator();
			CameraMode = new ThirdPersonCamera();

			Clothing.DressEntity( this );

			Inventory.Add( new Shotgun(), true );
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
			MvmGame.Instance.PPSetSaturation( 1 );
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

			var controller = GetActiveController();
			controller?.Simulate( cl, this, GetActiveAnimator() );

			if ( Input.ActiveChild != null )
			{
				ActiveChild = Input.ActiveChild;
			}

			SimulateActiveChild( cl, ActiveChild );
		}

		public override void TakeDamage( DamageInfo info )
		{
			if ( info.Attacker == null ) return;
			if ( info.Attacker.GetType() == typeof( HumanPlayer ) ) return; //Simple no friendly fire
			base.TakeDamage( info );
		}

		public override void OnKilled()
		{
			timeSinceDied = 0;
			EnableDrawing = false;
			EnableAllCollisions = false;
			Inventory.DeleteContents();
			base.OnKilled();
		}

		protected override void OnDestroy()
		{
			hudPanel?.Delete();
			base.OnDestroy();
		}

	}

}
