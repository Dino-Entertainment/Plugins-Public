using System;
using MCGalaxy.Events;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Modules.Relay.Discord;

namespace MCGalaxy
{

	public class PluginAlertClient : Plugin
	{
		public override string creator { get { return "SonicDude"; } } //made from KickJini.cs
		public override string MCGalaxy_Version { get { return "1.9.0.0"; } }
		public override string name { get { return "AlertClient"; } }

		public override void Load(bool startup)
		{
			OnPlayerConnectEvent.Register(AlertClient, Priority.High);
		}

		public override void Unload(bool shutdown)
		{
			OnPlayerConnectEvent.Unregister(AlertClient);
		}

		void AlertClient(Player p)
		{
			string app = p.appName;
			if (p.Session.ProtocolVersion == Server.VERSION_0016)
			{
				Logger.Log(LogType.UserActivity, "{0} connected using Protocol 3 (c0.0.16a_02).", p.name);
				Chat.MessageFrom(p, "Connected using Protocol 3 (c0.0.16a_02).");
				DiscordPlugin.Bot.SendPublicMessage("**Protocol:** 3 (c0.0.16a_02).");

			}
			if (p.Session.ProtocolVersion == Server.VERSION_0017)
			{
				Logger.Log(LogType.UserActivity, "{0} connected using Protocol 4 (c0.0.17a - c0.0.18a_02).", p.name);
				Chat.MessageFrom(p, "Connected using Protocol 4 (c0.0.17a - c0.0.18a_02).");
				DiscordPlugin.Bot.SendPublicMessage("**Protocol:** 4 (c0.0.17a - c0.0.18a_02).");
			}
			if (p.Session.ProtocolVersion == Server.VERSION_0019)
			{
				Logger.Log(LogType.UserActivity, "{0} connected using Protocol 5. (c0.0.19a - c0.0.19a_06)", p.name);
				Chat.MessageFrom(p, "Connected using Protocol 5. (c0.0.19a - c0.0.19a_06)");
				DiscordPlugin.Bot.SendPublicMessage("**Protocol:** 5. (c0.0.19a - c0.0.19a_06)");
			}
			if (p.Session.ProtocolVersion == Server.VERSION_0020)
			{
				Logger.Log(LogType.UserActivity, "{0} connected using Protocol 6. (c0.0.20a_01 - c0.27_st)", p.name);
				Chat.MessageFrom(p, "Connected using Protocol 6. (c0.0.20a_01 - c0.27_st)");
				DiscordPlugin.Bot.SendPublicMessage("**Protocol:** 6. (c0.0.20a_01 - c0.27_st)");
			}
			if (p.Session.ProtocolVersion == Server.VERSION_0030)
			{
				Logger.Log(LogType.UserActivity, "{0} connected using Protocol 7. (c0.28_01 - c0.30)", p.name);
				Chat.MessageFrom(p, "Connected using Protocol 7. (c0.28_01 - c0.30)");
				DiscordPlugin.Bot.SendPublicMessage("**Protocol:** 7. (c0.28_01 - c0.30)");
			}
			if (app == null || p.name.EndsWith(" +"))
			{
				Logger.Log(LogType.UserActivity, "{0} connected using the Classicube Desktop Client but in Classic Mode or some other Classic MC Client", p.name);
				Chat.MessageFrom(p, "Connected using the Classicube Desktop Client but in Classic Mode or some other Classic MC Client");
				DiscordPlugin.Bot.SendPublicMessage("``" + p.name + " connected using the Classicube Desktop Client but in Classic Mode or some other Classic MC Client``");
			}
			if (app == null || !p.name.EndsWith(" +"))
			{
				Logger.Log(LogType.UserActivity, "{0} connected using the Betacraft Client", p.name);
				Chat.MessageFrom(p, "Connected using the Betacraft Client");
				DiscordPlugin.Bot.SendPublicMessage("``" + p.name + " connected using the Betacraft Client``");
			}
			if (app != null && !app.EndsWith(" web") && !app.EndsWith(" mobile") && !app.EndsWith(" android alpha") && p.ip != "5.182.206.84")
			{
				Logger.Log(LogType.UserActivity, "{0} connected using the Classicube Desktop Client", p.name);
				Chat.MessageFrom(p, "Connected using the Classicube Desktop Client");
				DiscordPlugin.Bot.SendPublicMessage("``" + p.name + " connected using the Classicube Desktop Client``");
			}
			if (app.EndsWith(" web"))
			{
				Logger.Log(LogType.UserActivity, "{0} connected using the Classicube Web Client", p.name);
				Chat.MessageFrom(p, "Connected using the Classicube Web Client");
				DiscordPlugin.Bot.SendPublicMessage("``" + p.name + " connected usig the Classicube Web Client``");
			}
			if (app.EndsWith(" mobile"))
			{
				Logger.Log(LogType.UserActivity, "{0} connected via the Classicube Mobile Web Client", p.name);
				Chat.MessageFrom(p, "Connected using the Classicube Mobile Web Client");
				DiscordPlugin.Bot.SendPublicMessage("``" + p.name + " connected using the Classicube Mobile Web Client``");
			}
			if (app.EndsWith(" android alpha"))
			{
				Logger.Log(LogType.UserActivity, "{0} connected using the Classicube Mobile Client", p.name);
				Chat.MessageFrom(p, "Connected using the Classicube Mobile Client");
				DiscordPlugin.Bot.SendPublicMessage("``" + p.name + " connected using the Classicube Mobile Client``");
			}
			if (p.ip == "5.182.206.84" && app != null)
			{
				Logger.Log(LogType.UserActivity, "{0} connected using the Classicube Desktop Client via ViaProxy", p.name);
				Chat.MessageFrom(p, "Connected using the Classicube Desktop Client via ViaProxy");
				DiscordPlugin.Bot.SendPublicMessage("``" + p.name + " connected using the Classicube Desktop Client via ViaProxy``");
			}
			if (p.ip == "5.182.206.84" && app == null && p.Session.ProtocolVersion == Server.VERSION_0030)
			{
				Logger.Log(LogType.UserActivity, "{0} connected using the Classic Desktop Client via ViaProxy", p.name);
				Chat.MessageFrom(p, "Connected using the Classic Desktop Client via ViaProxy");
				DiscordPlugin.Bot.SendPublicMessage("``" + p.name + " connected using the Classic Desktop Client via ViaProxy``");
			}
			if (p.ip == "5.182.206.84" && app == null || p.Session.ProtocolVersion != Server.VERSION_0030)
			{
				Logger.Log(LogType.UserActivity, "{0} connected using the Classic Client via ViaProxy", p.name);
				Chat.MessageFrom(p, "Connected using the Classic Desktop Client via ViaProxy");
				DiscordPlugin.Bot.SendPublicMessage("``" + p.name + " connected using the Classic Desktop Client via ViaProxy``");
			}
		}
	}
}
