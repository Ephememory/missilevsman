using System;
using System.Collections.Generic;
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
		public MissileLifeTimeBar( MissilePlayer ply )
		{
			player = ply;
			label = Add.Label();
			timeLeft = Game.MaxLifeTime - 1;
		}

		public override void Tick()
		{
			base.Tick();
			if ( player.LifeState != LifeState.Alive ) return;
			timeLeft -= Time.Delta;

			label.SetText( $"⏲️ {timeLeft.CeilToInt()}" );

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
