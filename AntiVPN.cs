using System;
using System.Net;

using MCGalaxy;
using MCGalaxy.Config;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Network;


public class PluginAntiVPN : Plugin
{
    public override string creator { get { return "SonicDude"; } }
    public override string MCGalaxy_Version { get { return "1.9.0.0"; } }
    public override string name { get { return "AntiVPN"; } }

    public override void Load(bool startup)
    {
        OnPlayerConnectEvent.Register(AntiVPN, Priority.High);
    }

    public override void Unload(bool shutdown)
    {
        OnPlayerConnectEvent.Unregister(AntiVPN);
    }

    void AntiVPN(Player p)
    {
        string ip = p.ip;

        string json, proxy = "N/A";
        using (WebClient client = HttpUtil.CreateWebClient())
        {
            json = client.DownloadString("http://ip-api.com/json/" + ip + "?fields=status,message,proxy,query");
        }

        JsonReader reader = new JsonReader(json);
        reader.OnMember = (obj, key, value) =>
        {
            if (key == "proxy") proxy = (string)value;
        };

        reader.Parse();

        if (proxy == "true" ) {
            p.Leave("A VPN has been detected, please disable it to join");
            p.cancellogin = true;
        }
    }
}
