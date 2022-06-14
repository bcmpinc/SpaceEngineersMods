using Sandbox.Game.Entities;
using Sandbox.Game.Gui;
using Sandbox.Game.Screens.Helpers;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game;
using VRage.Plugins;

namespace MoreEvents
{
    public class MoreEvents : IPlugin
    {
        public void Dispose() {
        }

        public void Init(object gameInstance) {
            var x = new MyToolbar(MyToolbarType.ButtonPanel, 5, 1);
            
        }

        public void Update() {
        }
    }
}
