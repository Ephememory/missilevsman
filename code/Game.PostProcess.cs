using Sandbox;

namespace Missile
{
	public partial class MvmGame : Sandbox.Game
	{
		private StandardPostProcess pp;
		private float saturation = 1f;

		public void PPSetSaturation( float amt )
		{
			Host.AssertClient();
			if ( amt > 1.0f ) amt = 1.0f;
			if (amt < 0.0f ) amt = 0.0f;
			saturation = amt;
			pp.Saturate.Enabled = true;
			pp.Saturate.Amount = saturation;
		}

		public void PPClearSaturation()
		{
			Host.AssertClient();

			pp.Saturate.Enabled = false;
			pp.Saturate.Amount = 0f;
		}

		public override void FrameSimulate( Client cl )
		{
			//Restore saturation to normal every frame.
			saturation = MathX.Clamp( saturation.Approach( 1, Time.Delta * 1.3f ), 0, 1 );
			pp.Saturate.Amount = saturation;

			base.FrameSimulate( cl );
		}
	}

}
