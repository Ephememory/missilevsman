using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Missile.UI
{
	[Library]
	public class Hud : HudEntity<RootPanel>
	{
		public Hud()
		{
			if ( !IsClient )
				return;

			RootPanel.StyleSheet.Load( "/UI/hud.scss" );

			RootPanel.AddChild<NameTags>();
			RootPanel.AddChild<ChatBox>();
			RootPanel.AddChild<VoiceList>();
			RootPanel.AddChild<KillFeed>();
			RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
			RootPanel.Add.Label( MvmGame.VERSION, "version" );
		}
	}
}
