using missile.Util;
using Missile.Camera;
using Sandbox;

namespace Missile.Player
{
	public partial class HumanPlayer : Sandbox.Player
	{

		public HumanPlayer()
		{
		}

		public override void Respawn()
		{
			base.Respawn();
			SetModel( "models/citizen/citizen.vmdl" );
			Controller = new WalkController();
			Animator = new StandardPlayerAnimator();
			Camera = new ThirdPersonCamera();

			EnableTouch = true;
			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;
		}

		public override void Simulate( Client cl )
		{
			if ( LifeState == LifeState.Dead )
			{
				if ( IsServer )
				{
					Respawn();
				}

				return;
			}

			var controller = GetActiveController();
			controller?.Simulate( cl, this, GetActiveAnimator() );

			if ( IsServer && Input.Pressed( InputButton.Use ) )
			{
				Missile.Game.Swap( this );
			}
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
