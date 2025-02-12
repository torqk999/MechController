using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;

namespace IngameScript {

    partial class Program {

        class Group : Root {
            IMyBlockGroup group;
            public IMyBlockGroup BlockGroup { get { return group; } }
            public List<IMyTerminalBlock> Blocks {
                get {
                    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
                    group.GetBlocks(blocks);
                    return blocks;
                }
            }
            public override string Name {
                get { return group == null ? "Missing Group!" : group.Name; }
                set { StaticDlog("Create new block group in terminal!\n"); }
            }

            /// <summary>
            /// Ambiguous variable for foot locking procedure (Joint Set: index of foot currently locked. Foot: index of keyFrame that load on lock)
            /// </summary>
            public int LockedIndex;
            public bool Locked = false;

            public Group(string input) : base(input) {

            }

            public Group(IMyBlockGroup group, int[] intData) : base(intData, null) {
                this.group = group;
                LockedIndex = intData[(int)PARAM_int.lIX];
            }

            protected override bool Load(string[] input) {
                if (!base.Load(input)) return false;
                try {
                    LockedIndex = int.Parse(input[(int)PARAM_custom.lIX]);
                    group = GetGridBlockGroup(input[(int)PARAM_custom.Name]);
                    return true;
                }
                catch { return false; }
            }

            protected override void saveData(string[] saveBuffer) {
                base.saveData(saveBuffer);
                saveBuffer[(int)PARAM_custom.lIX] = LockedIndex.ToString();
            }
        }

    }
}
