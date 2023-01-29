using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.Entities.Blocks;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Interfaces.Terminal;
using SpaceEngineers.Game.ModAPI;
using System.Collections.Generic;
using System.Text;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;

namespace ConfigurableActions {

  [MyEntityComponentDescriptor(typeof(MyObjectBuilder_PistonBase), false)]
  public class SmartPiston : MyGameLogicComponent {
    static bool controls_created = false;
    static MyStringId S(string value) => MyStringId.GetOrCompute(value);
    static bool True(IMyTerminalBlock _) => true;
    static MyTerminalControlComboBoxItem cbi<T>(T key) {
      return new MyTerminalControlComboBoxItem {
        Key = (int)(object)(key),
        Value = S(System.Enum.GetName(typeof(KeyNotFoundException), key))
      };
    }

    private IMyPistonBase block;

    enum Controller : int {
      None=0,
      Stepper=1,
      Joystick=2
    }

    public static void global_init() {
      if (controls_created) return;
      controls_created = true;

      var controller = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlCombobox,IMyPistonBase>("controller");
      controller.Title = S("Movement controller");
      controller.Tooltip = S("Tooltip");
      controller.Visible = True;
      controller.ComboBoxContent = (list) => { 
        list.Add(cbi(Controller.None));
        list.Add(cbi(Controller.Stepper));
        list.Add(cbi(Controller.Joystick));
      };
      controller.Getter = (block) => 0;
      controller.Setter = (block, value) => { };
      controller.SupportsMultipleBlocks = true;
      MyAPIGateway.TerminalControls.AddControl<IMyPistonBase>(controller);

      var max_speed = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlSlider,IMyPistonBase>("max_speed");
      max_speed.Title = S("Maximum velocity");
      max_speed.Tooltip = S("Tooltip");
      max_speed.Visible = True;
      max_speed.SupportsMultipleBlocks = true;
      max_speed.SetLimits(0.0f, 5.0f);
      max_speed.Getter = (block) => 5;
      max_speed.Setter = (block, value) => { };
      max_speed.Writer = (block, writer) => { writer.Append("Hello"); };
      MyAPIGateway.TerminalControls.AddControl<IMyPistonBase>(max_speed);
    }


    public override void Init(MyComponentDefinitionBase definition) {
      base.Init(definition);
      block = (IMyPistonBase)Entity;
      NeedsUpdate |= MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
    }
    public override void UpdateOnceBeforeFrame() {
      base.UpdateOnceBeforeFrame();
      if(block?.CubeGrid?.Physics == null) // ignore projected and other non-physical grids
        return;
      NeedsUpdate |= MyEntityUpdateEnum.EACH_10TH_FRAME;
    }

    public override void UpdateBeforeSimulation10() {
      base.UpdateBeforeSimulation10();
    }
  }
}
