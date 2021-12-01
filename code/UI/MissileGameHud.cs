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
			RootPanel.AddChild<NetworkInfo>();

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
			missilePlayerPanel?.Delete();
			humanPlayerPanel = new HumanPlayerPanel( human );
			RootPanel.AddChild( humanPlayerPanel );
			Log.Info( human );
			Log.Info( humanPlayerPanel );

		}

		private void MissileSpawned( MissilePlayer missile )
		{
			humanPlayerPanel?.Delete();
			missilePlayerPanel = new MissilePlayerPanel( missile );
			RootPanel.AddChild( missilePlayerPanel );
			Log.Info( missile );
		}
	}
}
