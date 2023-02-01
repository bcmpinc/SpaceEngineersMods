using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
  partial class Program : MyGridProgram {
    const string TAG = "[damage]";

    readonly List<IMyTextPanel> screens = new List<IMyTextPanel>();

    public bool is_tagged(IMyTextPanel block) {
      return block.CustomName.Contains(TAG);
    }

    public Program() {
      Runtime.UpdateFrequency = UpdateFrequency.Update100;
      GridTerminalSystem.GetBlocksOfType(screens, is_tagged);
      screens.ForEach(x => {
        x.ContentType = ContentType.TEXT_AND_IMAGE;
        var r = new List<string>();
        x.GetSprites(r);
        x.CustomData = String.Join("\n",r);
      });
    }

    public void Main(string argument, UpdateType updateSource) {
    }
  }
}
