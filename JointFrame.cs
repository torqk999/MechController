//using Sandbox.ModAPI;

using EmptyKeys.UserInterface.Generated.DataTemplatesContracts_Bindings;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// UniqueID matches that of the Joint it is associated with. When loading the JointFrame,
        /// it requires the JointSet passed to it, to already have all of its Joints loaded.
        /// </summary>
        class JointFrame : Animation {
            public Joint Joint;
            public KeyFrame KeyFrame => (KeyFrame)Parent;

            public override string Name {
                get { return Joint?.Name; }
                set { if (Joint != null && Joint.FuncBlock != null) Joint.FuncBlock.CustomName = value; }
            }

            public JointFrame(Joint joint, bool snapping = true) : base((float)joint?.CurrentPosition(), joint.UniqueID) // Snapshot
            {
                Joint = joint;

                if (snapping)
                    MySetting.Change((int)MySetting.MyValue());
            }
            public JointFrame(string input, JointSet jointSet) : base(input)
            {
                Joint = jointSet.GetChildByID<Joint>(UniqueID);
            }
            public override void GenerateSetting(float init)
            {
                Static($"jFrame {Name} GeneratingSetting...\n");
                MySetting = new Setting("Joint Position", "The animation value of the associated joint within a given keyFrame.",
                    init, Snapping ? 1 : 0.1f,
                    (Joint == null ? 0 : Joint.LimitMax()),
                    (Joint == null ? 0 : Joint.LimitMin()),
                    SnappingIncrement);
            }
            public void ChangeStatorLerpPoint(float value)
            {
                MySetting.Change(Joint.ClampTargetValue(value));
            }
        }

    }
}
