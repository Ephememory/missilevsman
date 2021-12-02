using Missile.Player;
using Missile.Util;
using Sandbox;

namespace Missile.Camera
{
	public class MissileCamera : Sandbox.Camera
	{
		private AnimEntity pawn;
		private float fovApproachAmount, fovApproachTime = 0;

		public void DoClientRespawn()
		{
			FieldOfView = 130;
			fovApproachAmount = 90;
			fovApproachTime = 1;
		}

		public void DoClientKilled()
		{
			fovApproachAmount = 40;
			fovApproachTime = 2;
		}

		public override void Activated()
		{
			base.Activated();
		}

		public override void Update()
		{
			pawn = Local.Pawn as AnimEntity;

			if ( pawn == null )
				return;

			Position = pawn.Position + (Input.Rotation.Backward * 200) + (Vector3.Up * 50f);
			Rotation = Input.Rotation;
			FieldOfView = MathX.LerpTo( FieldOfView, fovApproachAmount, Time.Delta * fovApproachTime );
			// FieldOfView = FieldOfView.Approach( fovApproachAmount, Time.Delta * fovApproachTime );

			Viewer = null;
		}


	}
}
