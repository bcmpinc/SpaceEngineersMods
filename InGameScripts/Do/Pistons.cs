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
    class Pistons {
      private static readonly Dictionary<IMyPistonBase, float> Targets = new Dictionary<IMyPistonBase, float>();
      public static void Clear() => Targets.Clear();
      public static int Active { get { return Targets.Count; } }

      public static void Move(IMyPistonBase piston, float length, float speed) {
        if (piston.CustomName.Contains("[inv]")) length = piston.MaxLimit - length;
        Targets[piston] = length;
        float delta = length - piston.CurrentPosition;
        if (piston.MinLimit > length) piston.MinLimit = length;
        if (piston.MaxLimit < length) piston.MaxLimit = length;
        piston.Velocity = Math.Sign(delta) * speed;
      }

      public static void TickUpdate() {
        foreach (var rt in Targets.ToList()) {
          float delta = rt.Value - rt.Key.CurrentPosition;
          var speed = rt.Key.Velocity;
          if (delta / speed < 1) speed = delta / 1;
          rt.Key.Velocity = speed;
          if (Math.Abs(speed) < 0.002) {
            rt.Key.Velocity = 0;
            Targets.Remove(rt.Key);
          }
        }
      }
    }
  }
}
