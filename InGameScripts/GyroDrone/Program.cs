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

    IMyShipController       controller;
    List<IMyGyro>           gyros      = new List<IMyGyro>();
    List<IMyThrust>         thrusters  = new List<IMyThrust>();

    public bool same_subgrid(IMyTerminalBlock block) {
      return Me.CubeGrid == block.CubeGrid;
    }

    public void assert(bool check, string error) {
      if (check) return;
      Echo("ERROR: " + error);
      Runtime.UpdateFrequency = 0;
      throw new Exception();
    }

    public Program() {
      List<IMyShipController> controllers = new List<IMyShipController>();
      
      GridTerminalSystem.GetBlocksOfType(gyros,       same_subgrid); assert(gyros.Count > 0, "Missing gyros");
      GridTerminalSystem.GetBlocksOfType(thrusters,   same_subgrid); assert(gyros.Count > 0, "Missing thrusters");
      GridTerminalSystem.GetBlocksOfType(controllers, same_subgrid); assert(gyros.Count > 0, "Missing ship controllers");

      controller = controllers[0];

      Runtime.UpdateFrequency = UpdateFrequency.Update1;
    }

    public void Save() {
      // Called when the program needs to save its state. Use
      // this method to save your state to the Storage field
      // or some other means. 
      // 
      // This method is optional and can be removed if not
      // needed.
    }

    public void Main(string argument, UpdateType updateSource) {
      var position = controller.GetPosition();
      var gravity = controller.GetNaturalGravity();
      
      var thrust = thrusters.Sum(x=>x.MaxEffectiveThrust);
      var mass = controller.CalculateShipMass().PhysicalMass;
      var max_lift = thrust / mass;

      var fraction = (float)(gravity.Length() / max_lift);

      thrusters.ForEach(x => x.ThrustOverridePercentage = fraction);
    }
  }
}
