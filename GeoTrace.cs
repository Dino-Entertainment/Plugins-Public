//reference System.dll
using System;
using System.Net;

using MCGalaxy;
using MCGalaxy.DB;
using MCGalaxy.Events;
using MCGalaxy.Config;
using MCGalaxy.Network;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Commands.Moderation;
using MCGalaxy.Modules.Relay.Discord;


namespace Core
{

    public class PluginGeoTrace : Plugin
    {
        public override string creator { get { return "SonicDude"; } }
        public override string MCGalaxy_Version { get { return "1.9.0.0"; } }
        public override string name { get { return "GeoTrace"; } }

        public override void Load(bool startup)
        {
            OnPlayerConnectEvent.Register(GeoTrace, Priority.High);
            Command.Register(new CmdGeoTrace());
        }

        public override void Unload(bool shutdown)
        {
            OnPlayerConnectEvent.Unregister(GeoTrace);
            Command.Unregister(Command.Find("GeoTrace"));
        }

        void GeoTrace(Player p)
        {
            string msg = "λNICK &Sis using a private IP";

            string name = p.name;
            string ip = p.ip;
            ItemPerms opchat = Chat.OpchatPerms;

            if (ip == null) return;

            if (IPUtil.IsPrivate(IPAddress.Parse(ip)))
            {
                msg = msg.Replace("λNICK", name);

                Chat.MessageFrom(p, msg,
                                (pl, obj) => pl.CanSee(p) && opchat.UsableBy(pl.Rank));
            }

            string json, asn = "N/A", city = "N/A", continent = "N/A", iso = "N/A", country = "N/A", region = "N/A", proxy = "N/A", proxylikeness = "N/A", type = "N/A", isp = "N/A", org = "N/A", hosting = "N/A", mobile = "N/A", ascode = "N/A"; ;
            using (WebClient client = HttpUtil.CreateWebClient())
            {
                json = client.DownloadString("http://ip-api.com/json/" + ip + "?fields=status,message,continent,continentCode,country,countryCode,region,regionName,city,district,zip,lat,lon,timezone,isp,org,as,asname,reverse,mobile,proxy,hosting,query");
            }

            JsonReader reader = new JsonReader(json);
            reader.OnMember = (obj, key, value) =>
            {
                if (key == "asname") asn = (string)value;
                if (key == "city") city = (string)value;
                if (key == "country") country = (string)value;
                if (key == "continent") continent = (string)value;
                if (key == "region") region = (string)value;
                if (key == "proxy") proxy = (string)value;
                if (key == "mobile") mobile = (string)value;
                if (key == "hosting") hosting = (string)value;
                if (key == "type") type = (string)value;
                if (key == "as") ascode = (string)value;
                if (key == "isp") isp = (string)value;
                if (key == "org") org = (string)value;
                if (key == "continentCode") iso = (string)value;
            };

            reader.Parse();
            if (reader.Failed)
            {
                msg = "&SError parsing GeoIP info about λNICK.";
                msg = msg.Replace("λNICK", p.ColoredName);
                Chat.MessageFrom(p, msg, (pl, obj) => pl.CanSee(p) && opchat.UsableBy(pl.Rank));
                return;
            }

            msg = "λNICK &Sconnected from λCITY, λREGION, λCOUNTRY.";
            msg = msg
                .Replace("λNICK", p.ColoredName)
                .Replace("λCITY", city)
                .Replace("λREGION", region)
                .Replace("λCOUNTRY", country);


            Logger.Log(LogType.UserActivity, p.ColoredName + " &Sconnected to the server from " + city + ", " + region + ", " + country + ".");

            Chat.MessageFrom(p, msg, (pl, obj) => pl.CanSee(p) && opchat.UsableBy(pl.Rank));
            if (proxy == "true" && hosting == "true")
            {
                msg = "&WWarning&S: λNICK &Sis likely using a proxy on a VPS or Dedicated Server.";
                msg = msg.Replace("λNICK", p.ColoredName);
                Logger.Log(LogType.UserActivity, p.ColoredName + " &Sis suspected using a proxy on a VPS or Dedicated Server..");
                Chat.MessageFrom(p, msg, (pl, obj) => pl.CanSee(p) && opchat.UsableBy(pl.Rank));
                DiscordPlugin.Bot.SendStaffMessage(msg);
                return;
            }
            else if (proxy == "false" && hosting == "true")
            {
                msg = "&WWarning&S: λNICK &Sis using a VPS or Dedicated Server.";
                msg = msg.Replace("λNICK", p.ColoredName);
                Logger.Log(LogType.UserActivity, p.ColoredName + " &Sis suspected using a VPS or Dedicated Server..");
                Chat.MessageFrom(p, msg, (pl, obj) => pl.CanSee(p) && opchat.UsableBy(pl.Rank));
                DiscordPlugin.Bot.SendStaffMessage(msg);
                return;
            }
            else if (proxy == "true" && hosting == "false")
            {
                msg = "&WWarning&S: λNICK &Sis using a proxy on what is likely to be a residential connection.";
                msg = msg.Replace("λNICK", p.ColoredName);
                Logger.Log(LogType.UserActivity, p.ColoredName + " &Sis suspected to be using a proxy on what is likely to be a residential connection.");
                Chat.MessageFrom(p, msg, (pl, obj) => pl.CanSee(p) && opchat.UsableBy(pl.Rank));
                DiscordPlugin.Bot.SendStaffMessage(msg);
                return;
            }
            else
            {
                msg = "λNICK &Sis using a regular IP.";
                msg = msg.Replace("λNICK", p.ColoredName);
                Logger.Log(LogType.UserActivity, p.ColoredName + " &Sis using a regular IP.");
                Chat.MessageFrom(p, msg, (pl, obj) => pl.CanSee(p) && opchat.UsableBy(pl.Rank));
                DiscordPlugin.Bot.SendStaffMessage(msg);
                return;
            }

        }

        public class CmdGeoTrace : Command2
        {
            public override string name { get { return "geotrace"; } }
            public override string type { get { return CommandTypes.Moderation; } }
            public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

            public override void Use(Player p, string message, CommandData data)
            {
                if (message.Length == 0) { Help(p); return; }

                string ip = FindIP(p, message, "Proxy", out ip);
                if (ip == null) return;

                if (IPUtil.IsPrivate(IPAddress.Parse(ip)))
                {
                    p.Message("&WPlayer has an internal IP, cannot trace"); return;
                }

                string json, asn = "N/A", city = "N/A", continent = "N/A", iso = "N/A", country = "N/A", region = "N/A", proxy = "N/A", proxylikeness = "N/A", type = "N/A", isp = "N/A", org = "N/A", hosting = "N/A", mobile = "N/A", ascode = "N/A";
                using (WebClient client = HttpUtil.CreateWebClient())
                {
                    json = client.DownloadString("http://ip-api.com/json/" + ip + "?fields=status,message,continent,continentCode,country,countryCode,region,regionName,city,district,zip,lat,lon,timezone,isp,org,as,asname,reverse,mobile,proxy,hosting,query");
                }

                JsonReader reader = new JsonReader(json);
                reader.OnMember = (obj, key, value) =>
                {
                    if (key == "asname") asn = (string)value;
                    if (key == "city") city = (string)value;
                    if (key == "country") country = (string)value;
                    if (key == "continent") continent = (string)value;
                    if (key == "region") region = (string)value;
                    if (key == "proxy") proxy = (string)value;
                    if (key == "mobile") mobile = (string)value;
                    if (key == "hosting") hosting = (string)value;
                    if (key == "type") type = (string)value;
                    if (key == "as") ascode = (string)value;
                    if (key == "isp") isp = (string)value;
                    if (key == "org") org = (string)value;
                    if (key == "continentCode") iso = (string)value;
                };

                reader.Parse();
                if (reader.Failed) { p.Message("&WError parsing GeoIP info"); return; }

                if (proxy == "false" && hosting == "true")
                {
                    proxylikeness = "&4BOT HIGHLY LIKELY!!";
                }
                else if (proxy == "true" && hosting == "false")
                {
                    proxylikeness = "&4RESIDENTIAL VPN/PROXY HIGHLY LIKELY!!";
                }
                else if (proxy == "true" && hosting == "true")
                {
                    proxylikeness = "&4VPN/PROXY HIGHLY LIKELY!!";
                }
                else if (proxy == "false" && hosting == "false")
                {
                    proxylikeness = "&2UNLIKELY TO BE A THREAT";
                }

                p.Message("&SInformation about &T" + ip + "&S:");
                p.Message("&S· ASN: &T" + asn);
                p.Message("&S· Location: &T" + city + ", " + region + ", " + country + ", " + continent + " (" + iso + ")");
                p.Message("&S· ISP: &T" + isp);
                p.Message("&S· ISP ORG: &T" + org);
                p.Message("&S· Proxy: &T" + proxy.Capitalize());
                p.Message("&S· Mobile: &T" + mobile.Capitalize());
                p.Message("&S· Hosting: &T" + hosting.Capitalize());
                p.Message("&S· Proxy: " + proxylikeness);

            }

            static string FindIP(Player p, string message, string cmd, out string name)
            {
                IPAddress ip;
                name = null;

                // TryParse returns "0.0.0.123" for "123", we do not want that behaviour
                if (IPAddress.TryParse(message, out ip) && message.Split('.').Length == 4)
                {
                    string account = Server.Config.ClassicubeAccountPlus ? message + "+" : message;
                    if (PlayerDB.FindName(account) == null) return message;

                    // Some classicube.net accounts can be parsed as valid IPs, so warn in this case.
                    p.Message("Note: \"{0}\" is both an IP and an account name. "
                              + "If you meant the account, use &T/{1} @{0}", message, cmd);
                    return message;
                }

                if (message[0] == '@') message = message.Remove(0, 1);
                Player who = PlayerInfo.FindMatches(p, message);
                if (who != null) { name = who.name; return who.ip; }

                p.Message("Searching PlayerDB..");
                string dbIP;
                name = PlayerDB.FindOfflineIPMatches(p, message, out dbIP);
                return dbIP;
            }

            public override void Help(Player p)
            {
                p.Message("&T/Proxy [player/ip]");
                p.Message("&HOutputs information on how likely an IP is to be a proxy.");
            }
        }
    }
}