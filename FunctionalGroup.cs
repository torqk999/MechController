using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;

namespace IngameScript {

    partial class Program {

        class FunctionalGroup : Root {

            public IMyBlockGroup BlockGroup { get; private set; }

            public List<IMyTerminalBlock> Blocks {
                get {
                    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                    if (BlockGroup != null)
                        BlockGroup.GetBlocks(blocks);
                    return blocks;
                }
            }
            public override string Name {
                get { return BlockGroup == null ? "Missing Group!" : BlockGroup.Name; }
                set { StaticDlog("Create new block group in terminal!\n"); }
            }

            public bool Locked = false;

            public FunctionalGroup(string input) : base(input) {

            }

            public FunctionalGroup(IMyBlockGroup group, int uniqueID) : base(uniqueID) {
                this.BlockGroup = group;
            }

            protected override bool Load(string[] input) {
                if (!base.Load(input)) return false;
                    BlockGroup = GetGridBlockGroup(input[(int)SaveDataAttribute.Name]);
                return true;
            }

            protected override void saveData(string[] saveBuffer) {
                base.saveData(saveBuffer);
                saveBuffer[(int)SaveDataAttribute.Name] = Name;
            }
        }

    }
}
