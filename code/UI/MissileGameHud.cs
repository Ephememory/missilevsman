using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Missile.Player;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Missile.UI
{
	[Library]
	public class MissileGameHud : HudEntity<RootPanel>
	{
		private HumanPlayerPanel humanPlayerPanel = null;
		private MissilePlayerPanel missilePlayerPanel = null;

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
			// RootPanel.AddChild<NetworkInfo>();

			//PanelEvents didnt work for some reason
			HumanPlayer.OnSpawned += HumanSpawned;
			MissilePlayer.OnSpawned += MissileSpawned;
		}

		protected override void OnDestroy()
		{
			HumanPlayer.OnSpawned -= HumanSpawned;
			MissilePlayer.OnSpawned -= MissileSpawned;
			base.OnDestroy();
		}

		private void HumanSpawned( HumanPlayer human )
		{
			Clear();
			humanPlayerPanel = new HumanPlayerPanel( human );
			RootPanel.AddChild( humanPlayerPanel );
		}

		private void MissileSpawned( MissilePlayer missile )
		{
			Clear();
			missilePlayerPanel = new MissilePlayerPanel( missile );
			RootPanel.AddChild( missilePlayerPanel );
		}

		private void Clear()
		{
			missilePlayerPanel?.Delete();
			humanPlayerPanel?.Delete();
		}
	}
}
