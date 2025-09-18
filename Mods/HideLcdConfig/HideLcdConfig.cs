using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.Entities.Blocks;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces.Terminal;
using System.Collections.Generic;
using System.Text;
using VRage.Game.Components;
using VRage.ObjectBuilders;
using VRage.Utils;

namespace HideLcdConfig {
  [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
  public class HideLcdConfig : MySessionComponentBase {
    readonly HashSet<string> Filtered = new HashSet<string>("Title,Rotate,PanelList,Content,Script,ScriptForegroundColor,ScriptBackgroundColor,ShowTextPanel,Font,FontSize,FontColor,alignment,TextPaddingSlider,BackgroundColor,ImageList,SelectTextures,ChangeIntervalSlider,SelectedImageList,RemoveSelectedTextures,PreserveAspectRatio".Split(','));
    IMyTerminalControl[] AddedControls;
    bool ShowState = false;

    static bool HasLcd(IMyTerminalBlock block) => (block as Sandbox.ModAPI.Ingame.IMyTextSurfaceProvider)?.SurfaceCount > 0;

    public override void LoadData() {
      var Separate = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlSeparator, IMyTerminalBlock>("");
      var Control = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlOnOffSwitch, IMyTerminalBlock>("ShowLcdConfig");
      Control.Visible = x => true;
      Control.Getter = x => ShowState;
      Control.Setter = (x, v) => {
        ShowState = v;
        // HACK: Would be neater if I could call `RaiseVisibilityChanged()` directly, but that's not exposed in the Mod API.
        x.ShowInToolbarConfig ^= true;
        x.ShowInToolbarConfig ^= true;
      };
      Control.Title = MyStringId.GetOrCompute("Show LCD Controls");
      Control.OnText = MyStringId.GetOrCompute("On");
      Control.OffText = MyStringId.GetOrCompute("Off");
      MyAPIGateway.TerminalControls.CustomControlGetter += TerminalControls_CustomControlGetter;
      AddedControls = new IMyTerminalControl[] { Separate, Control };
    }

    public void TerminalControls_CustomControlGetter(IMyTerminalBlock block, List<IMyTerminalControl> controls) {
      if (HasLcd(block)) {
        var index = controls.FindIndex(x => x.Id == "PanelList");
        if (index >= 0) {
          controls.InsertRange(index, AddedControls);
        } else { 
          // Assume PanelList is missing because this mod is loaded twice and the changes already applied.
          return; 
        }

        if (!ShowState) {
          string last = null;
          for (int i = 0; i < controls.Count; i++) {
            var id = controls[i].Id;
            if (Filtered.Contains(id)) {
              controls[i] = null;
            } else if (id == last) {
              controls[i] = null;
            } else {
              last = id;
            }
          }
          controls.RemoveAll(x => x == null);
        }
      }
    }
  }
}
