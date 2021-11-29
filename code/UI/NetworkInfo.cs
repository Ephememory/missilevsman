
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class NetworkInfo : Panel
{
	Label label;
	public NetworkInfo()
	{
		label = Add.Label( "", "value" );
		label.Style.FontColor = Global.IsListenServer ? Color.FromBytes( 240, 240, 255, 230) : Color.FromBytes(255, 240, 240, 220);
		label.SetText( Global.IsListenServer ? "Server" : "Client" );

	}

}
