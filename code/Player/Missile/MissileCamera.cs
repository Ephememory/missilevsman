using Sandbox;

namespace Missile.Camera
{
	public class MissileCamera : Sandbox.Camera
	{
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
			Rotation = Input.Rotation;

			FieldOfView = 90;
			Viewer = null;
		}


	}
}
