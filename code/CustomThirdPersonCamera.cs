using Sandbox;

namespace Missile.Camera
{
	public class CustomThirdPersonCamera : Sandbox.Camera
	{

		private float orbitDistance = 130f;
		private Angles orbitAngles;
		private bool orbitMode = false;

		private AnimEntity pawn;

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
			Rotation = Rotation.Slerp(Rotation, Rotation.LookAt( pawn.Rotation.Forward ), 16f * Time.Delta);


			FieldOfView = 90;
			Viewer = null;
		}


	}
}
