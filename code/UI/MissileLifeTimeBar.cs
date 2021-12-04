using Missile.Player;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Missile.UI
{
	public class MissileLifeTimeBar : Panel
	{
		Label label;
		private MissilePlayer player;
		private float timeLeft;
		private string randomEmoji;
		
		public MissileLifeTimeBar()
		{
			string[] picks = { "⌛", "⏲️", "⌚", "⏱️", "⏰", "⏳" };
			randomEmoji = picks[Rand.Int( 0, picks.Length - 1 )];
			label = Add.Label();
			player = Local.Pawn as MissilePlayer;

			Reset();
		}
		
		public void Reset()
		{
			timeLeft = Game.MaxLifeTime - 1;
		}
		
		public override void Tick()
		{
			base.Tick();
			if ( player.LifeState != LifeState.Alive ) return;
			timeLeft -= Time.Delta;

			label.SetText( $"{randomEmoji} {timeLeft.CeilToInt()}" );

			if ( timeLeft <= 5 )
			{
				label.Style.FontColor = Color.Red;
				label.Style.Dirty();
			}
			else
			{
				label.Style.FontColor = Color.White;
				label.Style.Dirty();
			}

		}
	}
}
