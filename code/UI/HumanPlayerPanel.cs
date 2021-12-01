using System;
using System.Collections.Generic;
using Missile.Player;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Missile.UI
{
	public class HumanPlayerPanel : Panel
	{
		public HumanPlayerPanel( HumanPlayer player )
		{
			AddChild( new Health() );
			Add.Label( $"HUMAN : {player.Name}" );
		}
	}
}
