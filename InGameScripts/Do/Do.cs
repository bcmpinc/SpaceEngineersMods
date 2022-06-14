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
  partial class Program {
    public void Do<T>(string name, Action<T> action) where T : class, IMyTerminalBlock {
      var group = GridTerminalSystem.GetBlockGroupWithName(name);
      try {
        if (group == null) {
          action(GridTerminalSystem.GetBlockWithName(name) as T);
        } else {
          List<T> blocks = new List<T>();
          group.GetBlocksOfType(blocks);
          blocks.ForEach(action);
        }
      } catch (Exception e) {
        Echo("Error while performing action for '{0}':\n{1}", name, e);
      }
    }
  }
}
