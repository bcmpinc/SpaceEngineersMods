using System;
using Sandbox.Game.GameSystems.TextSurfaceScripts;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.Utils;
using VRageMath;


namespace DamageMonitor {
  [MyTextSurfaceScript("DamageMonitor", "Damage Monitor")]
  public class DamageMonitor : MyTSSCommon {
    public override ScriptUpdate NeedsUpdate => ScriptUpdate.Update100;
    private readonly IMyCubeGrid grid;


    public DamageMonitor(IMyTextSurface surface, IMyCubeBlock block, Vector2 size) : base(surface, block, size) {
      grid = block.CubeGrid;
    }

    void Draw() {
      var size = Surface.SurfaceSize;
      var frame = Surface.DrawFrame();
      var center = size / 2;
      var scale = 10f;
      for (var x = grid.Min.X; x <= grid.Max.X; x++) {
        for (var y = grid.Min.Y; y <= grid.Max.Y; y++) {
          bool block = false;
          bool damage = false;
          bool fat = false;
          bool broken = false;
          bool deform = false;
          for (var z = grid.Min.Z; z <= grid.Max.Z; z++) {
            if (grid.CubeExists(new Vector3I(x, y, z))) block = true;
            var c = grid.GetCubeBlock(new Vector3I(x, y, z));
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
            /**/
            if (broken) c = Color.Red;
            else if (damage && fat) c = Color.Orange;
            else if (damage) c = Color.Yellow;
            else if (deform) c = Color.Gray;
            else if (fat) c = Color.Blue;

            var s = MySprite.CreateSprite("SquareSimple", center + new Vector2(x, -y) * scale, new Vector2(8, 8));
            s.Color = c;
            frame.Add(s);
          }
        }
      }
      frame.Dispose();
    }

    public override void Run() {
      try {
        base.Run(); // do not remove
        Draw();
      } catch (Exception e) {
        MyLog.Default.WriteLineAndConsole($"{e.Message}\n{e.StackTrace}");
        var text = $"ERROR: Damage Monitor Crashed!\n\n{e.Message}\n{e.StackTrace}\n\nPlease copy text of this error and sent to mod author.\n{MyAPIGateway.Utilities.GamePaths.ModScopeName}";
        Surface.ContentType = ContentType.TEXT_AND_IMAGE;
        Surface.WriteText(text);
      }
    }
  }
}
