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
    public class setAdm : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "setadm";

        public string Help => "";

        public string Syntax => "<player>";

        public List<string> Aliases => new List<string> { "ranisdolboeb" };

        public List<string> Permissions => new List<string> { "denzmele.setadm" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer up = (UnturnedPlayer)caller;
            if (command.Length == 1)
            {
                UnturnedPlayer up2 = UnturnedPlayer.FromName(command[0]);
                if (up2 != null)
                {
                    Plugin.playersData.Add(new Plugin.Player()
                    {
                        steamID = up2.CSteamID,
                        playedTime = 0,
                        lastCon = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                        IsOnline = true,
                        DisplayName = up2.DisplayName
                    });
                    Plugin.PlayersDataStorage.Save(Plugin.playersData);
                }
                else
                {
                    ChatManager.serverSendMessage("Игрок не найден", Color.red, null, up.SteamPlayer(), EChatMode.SAY, "https://i.imgur.com/RqEsRLC.png");
                }
            }
        }
    }
}
