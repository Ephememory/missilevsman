using Sandbox;

namespace Missile
{
	public partial class Game : Sandbox.Game
	{
		private StandardPostProcess pp;
		private float saturation = 100f;

		public void PPSetSaturation( float amt )
		{
			Host.AssertClient();

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
			//Resaturate the pp
			pp.Saturate.Enabled = true;
			pp.Saturate.Amount = saturation;
			saturation = saturation.Approach( 100f, Time.Delta );

			base.FrameSimulate( cl );
		}
	}

}
