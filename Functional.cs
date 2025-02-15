//using Sandbox.ModAPI;
using SpaceEngineers.Game.ModAPI.Ingame;
using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;
using VRage.GameServices;

namespace IngameScript
{
    partial class Program
    {
        class Functional : Root
        {
            public IMyFunctionalBlock FuncBlock { get; private set; }
            public int FootID;
            public Functional(IMyFunctionalBlock funcBlock) : base(funcBlock.CustomData) {
                this.FuncBlock = funcBlock;
            }
            public Functional(IMyFunctionalBlock funcBlock, int uniqueID, int footID = -1) : base(uniqueID) {
                this.FuncBlock = funcBlock;
                FootID = footID;
                TAG = ParseBlockTag(funcBlock);
            }

            public override string Name {
                get { return FuncBlock?.CustomName;}
                set { if (FuncBlock != null) FuncBlock.CustomName = value; }
            }

            // Should this be made static for later use?   >:-|
            string ParseBlockTag(IMyTerminalBlock block) {
                if (block is IMyLandingGear)
                    return MagnetTag;
                if (!(block is IMyMechanicalConnectionBlock))
                    return string.Empty;
                if (FootID == -1)
                    return JointTag;
                if (block.CustomName.Contains(ToeSignature))
                    return GripTag;
                if (block.CustomName.Contains(TurnSignature))
                    return TurnTag;
                return PlaneTag;
            }

            public bool IsAlive() {
                try { return FuncBlock.IsWorking; }
                catch { return false; }
            }

            protected override bool Load(string[] data) {
                if (!base.Load(data)) return false;

                try {
                    FootID = int.Parse(data[(int)SaveDataAttribute.FootID]);
                    TAG = ParseBlockTag(FuncBlock);
                    return true;
                }
                catch { return false; }
            }

            public bool Save()
            {
                if (FuncBlock == null)
                    return false;

                FuncBlock.CustomData = SaveData();
                return true;
            }
        }

    }
}
