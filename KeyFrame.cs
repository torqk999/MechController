//using Sandbox.ModAPI;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IngameScript
{
    partial class Program
    {

        class KeyFrame : Animation
        {
            public List<Root> Jframes = new List<Root>();
            /// <summary>
            /// Create
            /// </summary>
            /// <param name="init"></param>
            /// <param name="name"></param>
            public KeyFrame(float init, string name = null) : base(init, -1, name)
            {
                TAG = KframeTag;
            }
            /// <summary>
            /// Load
            /// </summary>
            /// <param name="input"></param>
            public KeyFrame(string input) : base(input)
            {

            }

            public override void AddChild<T>(T child) {
                if (child == null || !(child is JointFrame))
                    return;

                Jframes.Add(child);
                child.Parent = this;
            }

            public override T GetChildByIndex<T>(int index) {
                if (index < 0)
                    return null;
                if (typeof(T) == typeof(JointFrame))
                    return index >= Jframes.Count ? null : (T)Jframes[index];
                return null;
            }

            public override T GetChildByID<T>(int ID) {
                if (typeof(T) == typeof(JointFrame))
                    return (T)Jframes.Find(x => x.UniqueID == ID);
                return null;
            }

            public override void GenerateSetting(float init)
            {
                MySetting = new Setting("Frame Length", "The time that will be displaced between this frame, and the one an index ahead",
                    init, FrameIncrmentMag, FrameLengthCap, FrameLengthMin, FrameDurationIncrement); //Inverse for accelerated effect
            }
        }

    }
}
