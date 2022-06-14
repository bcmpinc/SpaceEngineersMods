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
    class Async {
      private static IEnumerator<double> Current = null;
      private static double _Timeout = 0;
      public static double Timeout { get { return _Timeout; } }
      public static bool Active { get { return Current != null; } }
      public static void Run(IEnumerable<double> New) {
        Current = New.GetEnumerator();
        _Timeout = 0;
      }

      public static void Advance(double TotalSeconds) {
        if (Active) {
          _Timeout -= TotalSeconds;
          if (_Timeout <= 0) {
            if (Current.MoveNext()) {
              _Timeout = Current.Current;
            } else {
              Current = null;
            }
          }
        }
      }
    }
  }
}
