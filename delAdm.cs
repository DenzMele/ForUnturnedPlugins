using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TimePlayed
{
    public class delAdm : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string> { };

        public List<string> Permissions => new List<string> { "denzmele.deladm" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer up = (UnturnedPlayer)caller;
            if (command.Length == 1)
            {
                UnturnedPlayer up2 = UnturnedPlayer.FromName(command[0]);
                if (up2 != null)
                {
                    var indexplayer = Plugin.playersData.FindIndex(s => s.steamID == up2.CSteamID);
                    if(indexplayer != -1)
                    {
                        Plugin.playersData.RemoveAt(indexplayer);
                    }
                    else
                    {
                        ChatManager.serverSendMessage("Игрок не администратор.", Color.red, null, up.SteamPlayer(), EChatMode.SAY, "https://i.imgur.com/RqEsRLC.png");
                    }
                    Plugin.PlayersDataStorage.Save(Plugin.playersData);
                }
                else
                {
                    ChatManager.serverSendMessage("Игрок не найден.", Color.red, null, up.SteamPlayer(), EChatMode.SAY, "https://i.imgur.com/RqEsRLC.png");
                }
            }
        }
    }
}
