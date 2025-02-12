//using Sandbox.ModAPI;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {

        class KeyFrame : Animation
        {
            public List<Root> Jframes = new List<Root>();

            public KeyFrame(float init, int[] intData, List<JointFrame> jFrames = null, string name = null) : base(init, name, intData)
            {
                TAG = KframeTag;
                if (jFrames != null) Jframes.AddRange(jFrames);
            }
            public KeyFrame(string input, List<JointFrame> buffer) : base(input)
            {
                Jframes.AddRange(buffer);
                //BUILT = Load(input);
            }

            public JointFrame GetJointFrameByJointIndex(int index)
            {
                return GetJointFrameByFrameIndex(Jframes.FindIndex(x => x.MyIndex == index));
            }

            public JointFrame GetJointFrameByFrameIndex(int index)
            {
                if (index < 0 || index >= Jframes.Count)
                    return null;
                return (JointFrame)Jframes[index];
            }
            public override void Sort(eRoot type = eRoot.DEFAULT)
            {
                Jframes.Sort(SORT);
            }
            public override void GenerateSetting(float init)
            {
                MySetting = new Setting("Frame Length", "The time that will be displaced between this frame, and the one an index ahead",
                    init, FrameIncrmentMag, FrameLengthCap, FrameLengthMin, FrameDurationIncrement); //Inverse for accelerated effect
            }
        }

    }
}
