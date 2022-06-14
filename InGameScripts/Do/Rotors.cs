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
    class Rotors {
      private static readonly Dictionary<IMyMotorStator, float> Targets = new Dictionary<IMyMotorStator, float>();
      public static void Clear() => Targets.Clear();
      public static int Active { get { return Targets.Count; } }

      public static void Lock(IMyMotorStator rotor, float angle, float speed) {
        rotor.RotorLock = false;
        Targets[rotor] = angle;
        float currentAngle = MathHelper.ToDegrees(rotor.Angle);
        float delta = angle - currentAngle;
        if (delta > 180) delta -= 360;
        if (delta < -180) delta += 360;
        rotor.TargetVelocityRPM = Math.Sign(delta) * speed;
      }

      public static void Unlock(IMyMotorStator rotor) {
        rotor.RotorLock = false;
        rotor.TargetVelocityRPM = 0;
        Targets.Remove(rotor);
      }

      public static void TickUpdate() {
        foreach (var rt in Targets.ToList()) {
          float currentAngle = MathHelper.ToDegrees(rt.Key.Angle);
          float delta = rt.Value - currentAngle;
          if (delta > 180) delta -= 360;
          if (delta < -180) delta += 360;
          var speed = rt.Key.TargetVelocityRPM;
          if (delta / speed < 5) speed = delta / 5;
          rt.Key.TargetVelocityRPM = speed;
          if (Math.Abs(speed) < 0.01) {
            rt.Key.TargetVelocityRPM = 0;
            rt.Key.RotorLock = true;
            Targets.Remove(rt.Key);
          }
        }
      }
    }
  }
}
