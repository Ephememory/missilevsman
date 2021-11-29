using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace missile.UI
{
	[Library]
	public class MissileGameHud : HudEntity<RootPanel>
	{

		public MissileGameHud()
		{
			if ( !IsClient )
				return;

			RootPanel.StyleSheet.Load( "/UI/hud.scss" );

			RootPanel.AddChild<NameTags>();
			RootPanel.AddChild<ChatBox>();
			RootPanel.AddChild<VoiceList>();
			RootPanel.AddChild<KillFeed>();
			RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
			RootPanel.AddChild<NetworkInfo>();
		}
	}
}
