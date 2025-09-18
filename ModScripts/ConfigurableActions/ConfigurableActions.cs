using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.Entities.Blocks;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Interfaces.Terminal;
using SpaceEngineers.Game.ModAPI;
using System.Collections.Generic;
using System.Text;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.ObjectBuilders;
using VRage.Utils;

namespace ConfigurableActions {
  [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
  public class ConfigurableActionsSession : MySessionComponentBase {
    const string SECTION = "ConfigurableActions";

    static public float getConfigValue(IMyTerminalBlock b, string name, float def) {
      var ini = new MyIni();
      if (!ini.TryParse(b.CustomData)) {
        // In case of parse error, return default.
        return def;
      }
      
      var val = ini.Get(SECTION, name);
      if (val.IsEmpty) {
        // In case of missing value, add to config and return default.
        ini.Set(SECTION, name, def);
        b.CustomData = ini.ToString();
      }

      return val.ToSingle(def);
    }

    public void log_actions<T>() {
      List<IMyTerminalAction> actions;
      //MyAPIGateway.TerminalActionsHelper.GetActions(typeof(T), actions);
      MyAPIGateway.TerminalControls.GetActions<T>(out actions);
      MyLog.Default.WriteLineAndConsole("Block " + typeof(T) + ":");
      foreach (var act in actions) {
        MyLog.Default.WriteLineAndConsole(" - " + act.Id + ": " + act.Name);
      }
    }

    static public float zapsmall(float value) {
      if (-1e-5 < value && value < 1e-5) return 0;
      return value;
    }

    static public void wrapper<T>(IMyTerminalAction act, System.Func<T,float,bool> applicator, string name, float def) where T : IMyTerminalBlock {
      var old = act.Action;
      act.Action = (IMyTerminalBlock b) => {
        if (b is T) {
          var t = (T)b;
          var value = getConfigValue(b, name, def);
          if (applicator(t, value)) {
            return;
          }
        }
        old(b);
      };
    }

    public override void BeforeStart() {
      List<IMyTerminalAction> actions;

      MyAPIGateway.TerminalControls.GetActions<IMyPistonBase>(out actions);
      foreach (var act in actions) {
        switch(act.Id) {
          case "IncreaseVelocity": wrapper(act, (IMyPistonBase b, float value) => {b.Velocity = zapsmall(b.Velocity + value); return true; }, "velocity step", 0.5f); break;
          case "DecreaseVelocity": wrapper(act, (IMyPistonBase b, float value) => {b.Velocity = zapsmall(b.Velocity - value); return true; }, "velocity step", 0.5f); break;
          case "ResetVelocity":    wrapper(act, (IMyPistonBase b, float value) => {b.Velocity = value; return true; }, "velocity reset", 0.0f); break;
        }
      }
      
      MyAPIGateway.TerminalControls.GetActions<IMyMotorAdvancedStator>(out actions);
      foreach (var act in actions) {
        switch(act.Id) {
          case "IncreaseVelocity": wrapper(act, (IMyMotorStator b, float value) => {b.TargetVelocityRPM = zapsmall(b.TargetVelocityRPM + value); return true; }, "velocity step", 3.0f); break; 
          case "DecreaseVelocity": wrapper(act, (IMyMotorStator b, float value) => {b.TargetVelocityRPM = zapsmall(b.TargetVelocityRPM - value); return true; }, "velocity step", 3.0f); break; 
          case "ResetVelocity":    wrapper(act, (IMyMotorStator b, float value) => {b.TargetVelocityRPM = value; return true; }, "velocity reset", 0.0f); break; 
        }
      }

      MyAPIGateway.TerminalControls.GetActions<IMyTimerBlock>(out actions);
      foreach (var act in actions) {
        switch(act.Id) {
          case "IncreaseTriggerDelay": wrapper(act, (IMyTimerBlock b, float value) => {if (value == 0) return false; b.TriggerDelay = System.Math.Max(1, b.TriggerDelay + value); return true; }, "delay step", 0.0f); break; 
          case "DecreaseTriggerDelay": wrapper(act, (IMyTimerBlock b, float value) => {if (value == 0) return false; b.TriggerDelay = System.Math.Max(1, b.TriggerDelay - value); return true; }, "delay step", 0.0f); break; 
        }
      }
    }
  }
}
