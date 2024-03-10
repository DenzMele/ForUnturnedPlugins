using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimePlayed
{
    public class cfg : IRocketPluginConfiguration
    {
        public string webhook;
        public void LoadDefaults()
        {
            webhook="0";
        }
    }
}
