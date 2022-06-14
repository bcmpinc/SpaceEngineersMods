

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Game.Entities.Blocks;
using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRageMath;

namespace SpaceEngineersMods
{
    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_MotorStator), false, "LargeBallJoint")]
    public class BallJoint : MyGameLogicComponent {
        IMyMotorStator Block;
        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            var x = MyAPIGateway.TerminalControls.CreateAction<IMyProgrammableBlock>("Test");
            Block = Entity as IMyMotorStator;
            NeedsUpdate = MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
        }
    }

    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_MotorRotor), false, "LargeBallSocket")]
    public class BallSocket : MyGameLogicComponent {
        IMyMotorRotor Block;
        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            Block = Entity as IMyMotorRotor;
            NeedsUpdate = MyEntityUpdateEnum.BEFORE_NEXT_FRAME;
        }
    }
}
