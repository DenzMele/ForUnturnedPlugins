using Newtonsoft.Json;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace TimePlayed
{
    public class DataStorage<T> where T : class
    {
        public string DataPath { get; private set; }
        public DataStorage(string dir, string fileName)
        {
            DataPath = Path.Combine(dir, fileName);
        }

        public void Save(T obj)
        {
            string objData = JsonConvert.SerializeObject(obj, Formatting.Indented);

            using (StreamWriter stream = new StreamWriter(DataPath, false))
            {
                stream.Write(objData);
            }
        }

        public T Read()
        {
            if (File.Exists(DataPath))
            {
                string dataText;
                using (StreamReader stream = File.OpenText(DataPath))
                {
                    dataText = stream.ReadToEnd();
                }
                return JsonConvert.DeserializeObject<T>(dataText);
            }
            else
            {
                return null;
            }
        }
    }
    public class Plugin : RocketPlugin<cfg>
    {
        public static DataStorage<List<Player>> PlayersDataStorage = new("Plugins/TimePlayed", "Admin.Data.json");
        public static List<Player> playersData = new();
        public static Plugin Instance;

        public class Player
        {
            public CSteamID steamID { get; set; }
            public long playedTime { get; set; }
            public long lastCon { get; set; }
            public string DisplayName { get; set; }
            public bool IsOnline { get; set; }
        }
        protected override void Load()
        {
            base.Load();
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
            InvokeRepeating("SetTime", 300f, 300f);
            InvokeRepeating("SendMSG", 1400f, 1400f);
            ReloadPlayersData();
            Instance = this;
        }

        public void SendMSG()
        {
            if (DateTime.UtcNow.Hour == 0)
            {
                string players = "";
                foreach (var playerR in playersData)
                {
                    players += $"Игрок {playerR.DisplayName} отыграл: {playerR.playedTime / 60} мин.\\n";
                    playerR.playedTime = 0;
                    PlayersDataStorage.Save(playersData);
                    ReloadPlayersData();
                }
                Logger.Log(players);
                string msg = $"{{ \"content\": null, \"embeds\": [ {{ \"title\": \"Онлайн игроков за {DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}:\", \"description\": \"{players}\", \"color\": 15269888 }} ], \"attachments\": [] }}";
                SendMs(msg);
            }
        }

        private void Events_OnPlayerDisconnected(Rocket.Unturned.Player.UnturnedPlayer player)
        {
            ReloadPlayersData();
            var index = playersData.FindIndex(s => s.steamID == player.CSteamID);
            if (index != -1)
            {
                playersData[index].IsOnline = false;
                PlayersDataStorage.Save(playersData);
                ReloadPlayersData();
            }
        }

        public void SetTime()
        {
            foreach (var player in playersData)
            {
                if (player.IsOnline)
                {
                    player.playedTime += DateTimeOffset.UtcNow.ToUnixTimeSeconds() - player.lastCon;
                    PlayersDataStorage.Save(playersData);
                    ReloadPlayersData();
                }
            }
        }


        private void Events_OnPlayerConnected(Rocket.Unturned.Player.UnturnedPlayer player)
        {
            player.Player.quests.sendAddGroupInvite(new CSteamID(103582791474257054));
            //player.Player.quests.SendAcceptGroupInvitation(new CSteamID(103582791474257054));
            var index = playersData.FindIndex(s => s.steamID == player.CSteamID);
            if (index != -1)
            {
                playersData[index].IsOnline = true;
                playersData[index].lastCon = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                PlayersDataStorage.Save(playersData);
                ReloadPlayersData();
            }
        }
        public void ReloadPlayersData()
        {
            playersData = PlayersDataStorage.Read();
            // If file didn't exist or was empty we have to initialize list to avoid null reference error
            if (playersData == null)
                playersData = new List<Player>();
        }

        public void AddPlayer(Player player)
        {
            // Adding player to playersData list
            playersData.Add(player);
            // Saving playersData list to file
            PlayersDataStorage.Save(playersData);
        }

        static void SendMs(string message)
        {
            string webhook = Plugin.Instance.Configuration.Instance.webhook;

            WebClient client = new WebClient();
            client.Headers.Add("Content-Type", "application/json");
            string payload = message;
            client.UploadData(webhook, Encoding.UTF8.GetBytes(payload));
        }
    }
}
