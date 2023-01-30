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
    public new void Echo(string format, params object[] args) => base.Echo(String.Format(format, args));

    IMyShipController controller;
    IMyTextSurface text;
    List<IMyGyro>     gyros      = new List<IMyGyro>();
    List<IMyThrust>   thrusters  = new List<IMyThrust>();

    public bool same_subgrid(IMyTerminalBlock block) {
      return Me.CubeGrid == block.CubeGrid;
    }

    public void assert(bool check, string error) {
      if (check) return;
      Echo("ERROR: {0}.", error);
      Runtime.UpdateFrequency = 0;
      throw new Exception();
    }

    public Program() {
      List<IMyShipController> controllers = new List<IMyShipController>();
      
      GridTerminalSystem.GetBlocksOfType(gyros,       same_subgrid); assert(gyros.Count > 0, "Missing gyros");
      GridTerminalSystem.GetBlocksOfType(thrusters,   same_subgrid); assert(gyros.Count > 0, "Missing thrusters");
      GridTerminalSystem.GetBlocksOfType(controllers, same_subgrid); assert(gyros.Count > 0, "Missing ship controllers");

      controller = controllers[0];
      text = Me.GetSurface(1);

      Runtime.UpdateFrequency = UpdateFrequency.Update1;
    }

    public void Save() {
      // Called when the program needs to save its state. Use
      // this method to save your state to the Storage field
      // or some other means. 
    }

    public void Main(string argument, UpdateType updateSource) {
      var mass = controller.CalculateShipMass().PhysicalMass;
      if (mass == 0) {
        thrusters.ForEach(x => x.ThrustOverridePercentage = 0);
        gyros.ForEach(x => x.Enabled = false);
        return;
      }

      var position = controller.GetPosition();
      var gravity = controller.GetNaturalGravity();
      var thrust = thrusters.Sum(x=>x.MaxEffectiveThrust);
      var max_lift = thrust / mass;

      var vel = controller.GetShipVelocities();
      gravity *= (1+controller.MoveIndicator.Y);
      gravity += vel.LinearVelocity * 3.0;

      var fraction = (float)(gravity.Length() / max_lift);
      text.WriteText(fraction.ToString());

      thrusters.ForEach(x => x.ThrustOverridePercentage = fraction);
      gyros.ForEach(x => { 
        //x.Yaw = controller.RollIndicator;
      });
    }
  }
}
