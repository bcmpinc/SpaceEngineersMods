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
    public IEnumerable<double> Handle(string arg) {
      switch (arg.ToLower()) {
        case "charge":
          Do<IMyPistonBase>("Launchpad - Charge Pistons", x => Pistons.Move(x, 0, 1f));
          var sensor = GridTerminalSystem.GetBlockWithName("Launchpad - Sensor") as IMySensorBlock;
          sensor.Enabled = true;
          yield return 0.5;

          var rocket = sensor.LastDetectedEntity;
          var worldToSensor = MatrixD.Invert(sensor.WorldMatrix);
          var position = Vector3D.Transform(rocket.Position, worldToSensor);
          var orientation = MatrixD.Multiply(rocket.Orientation, worldToSensor.GetOrientation());
          Echo("Name: {0}", rocket.Name);
          Echo("Position: {0}", position);
          sensor.Enabled = false;

          position.Y -= 14.25;
          position.Z = 0;
          var length = (float)(position.Length() - 0.2) / 4;
          var angle = (float)MathHelper.ToDegrees(Math.Atan2(position.X, -position.Y));
          Echo("Target length: {0:F2} angle: {1:F1}", length, angle);

          Do<IMyMotorStator>("Launchpad - Charge Rotor", x => Rotors.Lock(x, -angle, 1));
          Do<IMyMotorStator>("Launchpad - Charge Hinge", x => Rotors.Lock(x, angle, 1));
          Do<IMyPistonBase>("Launchpad - Charge Pistons", x => Pistons.Move(x, length, 0.5f));
          while (Rotors.Active > 0) yield return 1;
          Do<IMyMotorStator>("Launchpad - Charge Port", x => x.Attach());
          yield return 1;
          Do<IMyBatteryBlock>("Booster - Batteries", x => x.ChargeMode = ChargeMode.Recharge);
          Do<IMyMotorStator>("Launchpad - Charge Rotor", x => Rotors.Lock(x, 0, 1));
          Do<IMyMotorStator>("Launchpad - Charge Hinge", x => Rotors.Lock(x, 0, 1));
          Do<IMyPistonBase>("Launchpad - Charge Pistons", x => Pistons.Move(x, 14.25f/4, 1f));
          while (Rotors.Active > 0) yield return 1;
          Do<IMyLandingGear>("Booster - Landing", x => x.Lock());
          yield break;

        case "prelaunch":
          Do<IMyBatteryBlock>("Booster - Batteries", x => x.ChargeMode = ChargeMode.Auto);
          Do<IMyMotorStator>("Launchpad - Charge Port", x => x.Detach());
          Do<IMyMotorStator>("Launchpad - Charge Rotor", x => Rotors.Lock(x, 0, 1));
          Do<IMyMotorStator>("Launchpad - Charge Hinge", x => Rotors.Lock(x, 0, 1));
          Do<IMyPistonBase>("Launchpad - Charge Pistons", x => Pistons.Move(x, 0, 1f));
          yield break;

        case "reset":
          Do<IMyLandingGear>("Launchpad - Arm Magnetic Plate", x => x.Unlock());
          Do<IMyMotorStator>("Launchpad - Arm 0 Rotor", x => Rotors.Lock(x, 0, 0.5f));
          Do<IMyMotorStator>("Launchpad - Arm 1 Hinge", x => Rotors.Lock(x, -60, 0.5f));
          Do<IMyMotorStator>("Launchpad - Arm 2 Rotor", x => Rotors.Lock(x, 270, 0.5f));
          Do<IMyMotorStator>("Launchpad - Arm 3 Rotor", x => Rotors.Lock(x, 0, 1f));
          Do<IMyMotorStator>("Launchpad - Arm 4 Hinge", x => Rotors.Lock(x, -20, 1f));
          Do<IMyMotorStator>("Launchpad - Arm 5 Rotor", x => Rotors.Lock(x, 45, 1f));
          yield break;

        case "dock":
          Do<IMyShipConnector>("Booster - Connector", x => x.Disconnect());
          Do<IMyMotorStator>("Launchpad - Arm 0 Rotor", x => Rotors.Lock(x, 18.7f, 0.5f));
          Do<IMyMotorStator>("Launchpad - Arm 3 Rotor", x => Rotors.Lock(x, 0.1f, 1f));
          Do<IMyMotorStator>("Launchpad - Arm 4 Hinge", x => Rotors.Lock(x, -17.1f, 1f));
          Do<IMyMotorStator>("Launchpad - Arm 5 Rotor", x => Rotors.Lock(x, 16.9f, 1f));
          while (Rotors.Active > 0) yield return 1;

          Do<IMyMotorStator>("Launchpad - Arm 1 Hinge", x => Rotors.Lock(x, -10.1f, 0.5f));
          Do<IMyMotorStator>("Launchpad - Arm 2 Rotor", x => Rotors.Lock(x, 296.7f, 0.5f));
          while (Rotors.Active > 0) yield return 1;

          Do<IMyLandingGear>("Launchpad - Arm Magnetic Plate", x => x.Lock());
          Do<IMyShipConnector>("Launchpad - Dock Connector", x => x.Connect());
          yield break;

        case "booster":
          Do<IMyShipConnector>("Launchpad - Dock Connector", x => x.Disconnect());
          Do<IMyMotorStator>("Launchpad - Arm 0 Rotor", x => Rotors.Lock(x, 0, 0.5f));
          Do<IMyMotorStator>("Launchpad - Arm 1 Hinge", x => Rotors.Lock(x, -40, 0.5f));
          Do<IMyMotorStator>("Launchpad - Arm 2 Rotor", x => Rotors.Lock(x, 270, 0.5f));
          Do<IMyMotorStator>("Launchpad - Arm 3 Rotor", x => Rotors.Lock(x, 275.1f, 1f));
          Do<IMyMotorStator>("Launchpad - Arm 4 Hinge", x => Rotors.Lock(x, -22.0f, 1f));
          Do<IMyMotorStator>("Launchpad - Arm 5 Rotor", x => Rotors.Lock(x, 75.6f, 1f));
          yield return 3;
          Do<IMyMotorStator>("Launchpad - Arm 1 Hinge", x => Rotors.Lock(x, -22.8f, 0.5f));
          Do<IMyMotorStator>("Launchpad - Arm 2 Rotor", x => Rotors.Lock(x, 280.2f, 0.5f));
          while (Rotors.Active > 0) yield return 1;

          Do<IMyMotorStator>("Launchpad - Arm 0 Rotor", x => Rotors.Lock(x, 337.5f, 0.5f));
          while (Rotors.Active > 0) yield return 1;

          Do<IMyShipConnector>("Booster - Connector", x => x.Connect());
          yield break;

        default:
          Echo("Unknown task '{0}'", arg);
          yield break;
      }
    }

    public void Main(string args) {

      if (args != "") {
        Async.Run(Handle(args));
        Runtime.UpdateFrequency = UpdateFrequency.Update10;
      }

      Async.Advance(Runtime.TimeSinceLastRun.TotalSeconds);

      Rotors.TickUpdate();
      Pistons.TickUpdate();

      if (Rotors.Active > 0) Echo("{0} rotors", Rotors.Active);
      if (Pistons.Active > 0) Echo("{0} pistons", Pistons.Active);
      if (Async.Active) Echo("{0:f2}s", Async.Timeout);
      if (Rotors.Active == 0 && Pistons.Active == 0 && !Async.Active) {
        Echo("Finished");
        Runtime.UpdateFrequency = UpdateFrequency.None;
      }
    }
  }
}
