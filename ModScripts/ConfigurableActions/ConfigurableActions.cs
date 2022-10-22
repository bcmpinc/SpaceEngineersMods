using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.Entities.Blocks;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using System.Collections.Generic;
using System.Text;
using VRage.Game.Components;
using VRage.ObjectBuilders;
using VRage.Utils;

namespace ConfigurableActions {
  [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
  public class ConfigurableActions : MySessionComponentBase {
    public override void BeforeStart() {
      List<IMyTerminalAction> actions;
      MyAPIGateway.TerminalControls.GetActions<IMyPistonBase>(out actions);
      foreach (var act in actions) {
        switch(act.Id) {
          case "IncreaseVelocity":
            act.Action = b => (b as IMyPistonBase).Velocity += 0.2f;
            break;
          case "DecreaseVelocity":
            act.Action = b => (b as IMyPistonBase).Velocity -= 0.2f;
            break;
          case "ResetVelocity":
            act.Action = b => (b as IMyPistonBase).Velocity = 0.0f;
            break;
        }
      }
    }
    
  }
}
