//using Sandbox.ModAPI;
using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;
using VRage.GameServices;

namespace IngameScript
{
    partial class Program
    {
        class Functional : Root
        {
            IMyFunctionalBlock FuncBlock;
            public int FootIndex;
            public Functional(IMyFunctionalBlock funcBlock) : base(funcBlock.CustomData) {
                FuncBlock = funcBlock;
            }
            public Functional(IMyFunctionalBlock funcBlock, int[] intData) : base(intData) {
                TAG = ParseBlockTag(funcBlock, intData[(int)PARAM_int.fIX] > -1);
                FuncBlock = funcBlock;
            }

            public override string Name {
                get { return FuncBlock?.CustomName;}
                set { if (FuncBlock != null) FuncBlock.CustomName = value; }
            }

            public bool IsAlive() {
                try { return FuncBlock.IsWorking; }
                catch { return false; }
            }

            protected override bool Load(string[] data) {
                if (!base.Load(data)) return false;

                try {
                    FootIndex = int.Parse(data[(int)PARAM_custom.fIX]);
                    TAG = ParseBlockTag(FuncBlock, FootIndex > -1);
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
