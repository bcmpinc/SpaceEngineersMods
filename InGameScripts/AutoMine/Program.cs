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
    public new void Echo(string format, params object[] args) => base.Echo(String.Format(format, args));
    
    class DrillingRig {
      IMyCubeGrid root;

      List<IMyShipDrill>   drills;
      List<IMyMotorStator> stators = new List<IMyMotorStator>();
      List<IMyPistonBase>  pistons = new List<IMyPistonBase>();
      List<IMyMotorStator> axle    = new List<IMyMotorStator>();

      Vector3D axle_position;
      Vector3D axle_orientation;

      public DrillingRig(List<IMyShipDrill> _drills, Dictionary<IMyCubeGrid, List<IMyMechanicalConnectionBlock>> parent) {
        drills = _drills;
        /*
        var pars = new List<IMyMechanicalConnectionBlock>();
        pars.Add(parent[rig.Key].First());
        while (parent.ContainsKey(pars.Last().CubeGrid)) pars.Add(parent[pars.Last().CubeGrid].First());
        Echo(" - Rig at {0} with {1} drills", pars.Last().CustomName, rig.Value.Count);
        IMyTerminalBlock block = rig.Value.First();
        foreach(var line in pars) {
          var mat = MatrixD.Transpose(line.WorldMatrix.GetOrientation());
          var x = Vector3.Transform(block.WorldMatrix.Up, mat);
          Echo("     {0} at ({1:F3},{2:F3},{3:F3})", line.CustomName, x.X,x.Y,x.Z);
          block = line;
        }
        */
      }
    }

    List<DrillingRig> rigs = new List<DrillingRig>();

    public Program() {
      List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
      GridTerminalSystem.GetBlocks(blocks);

      // Gather connectivity data
      Dictionary<IMyCubeGrid, List<IMyMechanicalConnectionBlock>> parent = new Dictionary<IMyCubeGrid, List<IMyMechanicalConnectionBlock>>();
      foreach (var b in blocks) {
        if (b is IMyMechanicalConnectionBlock) {
          var grid = b.CubeGrid;
          if (!parent.ContainsKey(grid)) parent[grid] = new List<IMyMechanicalConnectionBlock>();
          var connector = b as IMyMechanicalConnectionBlock;
          parent[connector.TopGrid].Add(connector);
        }
      }

      // Detect Auto mining rigs
      Dictionary<IMyCubeGrid, List<IMyShipDrill>> mining_grids = new Dictionary<IMyCubeGrid, List<IMyShipDrill>>();
      foreach (var b in blocks) {
        if (b is IMyShipDrill) {
          var grid = b.CubeGrid;
          if (!mining_grids.ContainsKey(grid)) mining_grids[grid] = new List<IMyShipDrill>();
          mining_grids[grid].Add(b as IMyShipDrill);
        }
      }

      // Initialize rigs
      Echo("Found {0} drilling rigs:", mining_grids.Count);
      foreach (var rig in mining_grids) {
        rigs.Add(new DrillingRig(rig.Value, parent));
      }
    }

    public void Save() {
    }

    public void Main(string argument, UpdateType updateSource) {
    }
  }
}
