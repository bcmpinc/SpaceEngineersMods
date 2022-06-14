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
    
    // The drill shuts off when surpassing this fraction of inventory space.
    const float ShutoffFraction = 0.8f;

    List<IMyShipDrill> drills = new List<IMyShipDrill>();
    public Program() {
      Runtime.UpdateFrequency = UpdateFrequency.Update100;
      GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(drills);
    }

    public void Main(string args) {
      foreach (var d in drills) {
        if (!d.Closed && d.CubeGrid == Me.CubeGrid) {
          var inv = d.GetInventory();
          if (inv.CurrentVolume > inv.MaxVolume * ShutoffFraction) {
            d.Enabled = false;
          }
        }
      }
    }
  }
}
