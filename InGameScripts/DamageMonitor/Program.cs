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
    }

    void analyse_grid(Vector2 size, IMyCubeGrid grid, MySpriteDrawFrame frame) {
      var center = size/2;
      var scale = 10f;
      for (var x=grid.Min.X; x<=grid.Max.X; x++) {
        for (var y=grid.Min.Y; y<=grid.Max.Y; y++) {
          bool block  = false;
          bool damage = false;
          bool fat    = false;
          bool broken = false;
          bool deform = false;
          for (var z=grid.Min.Z; z<=grid.Max.Z; z++) {
            if (grid.CubeExists(new Vector3I(x,y,z))) block = true;
            var c = grid.GetCubeBlock(new Vector3I(x,y,z));
            if (c == null) continue;
            if (c.FatBlock != null) {
              fat = true;
              if (!c.FatBlock.IsFunctional) {
                broken = true;
              }
            }
            if (!c.IsFullIntegrity) damage = true;
            if (c.HasDeformation) deform = true;
          }

          if (block) { 
            Color c = Color.White;
            /**/ if (broken)        c = Color.Red;
            else if (damage && fat) c = Color.Orange;
            else if (damage)        c = Color.Yellow;
            else if (deform)        c = Color.Gray;
            else if (fat)           c = Color.Blue;

            var s = MySprite.CreateSprite("SquareSimple", center + new Vector2(x,-y) * scale, new Vector2(8,8));
            s.Color = c;
            frame.Add(s);
          }
        }
      }
    }

    public void Main(string argument, UpdateType updateSource) {
      screens.ForEach(x => {
        x.ContentType = ContentType.SCRIPT;
        x.Script = "";
        var size = x.SurfaceSize;
        var frame = x.DrawFrame();
        analyse_grid(size, x.CubeGrid, frame);
        frame.Dispose();
      });
    }
  }
}
