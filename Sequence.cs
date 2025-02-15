//using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {

        class Sequence : Animation
        {
            /// EXTERNALS ///
            public JointSet JointSet => (JointSet)Parent;
            public List<KeyFrame> Frames = new List<KeyFrame>();
            public KeyFrame[] CurrentFrames = new KeyFrame[2];

            // Logic
            public ClockMode RisidualClockMode = ClockMode.FOR;
            public ClockMode CurrentClockMode = ClockMode.PAUSE;
            public float CurrentClockTime = 0;
            public bool StepDelay;

            /// <summary>
            /// Create
            /// </summary>
            /// <param name="init"></param>
            /// <param name="uniqueID"></param>
            /// <param name="name"></param>
            public Sequence(float init, int uniqueID, string name = null) : base(init, uniqueID, name)
            {
                TAG = SeqTag;
            }
            /// <summary>
            /// Load
            /// </summary>
            /// <param name="input"></param>
            public Sequence(string input) : base(input)
            {

            }

            public override void AddChild<T>(T child) {
                if (child == null || !(child is KeyFrame))
                    return;

                Frames.Add(child as KeyFrame);
                child.Parent = this;
            }

            public override void AddChildren<T>(List<T> children) {
                if (children == null || !(children is List<KeyFrame>))
                    return;

                foreach (var child in children) {
                    Frames.Add(child as KeyFrame);
                    child.Parent = this;
                }
            }

            protected override bool Load(string[] data)
            {
                if (!base.Load(data))
                    return false;

                try { GenerateSetting(float.Parse(data[(int)SaveDataAttribute.SettingInit])); }
                catch { GenerateSetting(0); }

                return true;
            }
            public void OverrideSet()
            {
                foreach (Joint joint in JointSet.Joints)
                    joint.OverrideSequenceID = UniqueID;
            }

            public override void GenerateSetting(float init)
            {
                MySetting = new Setting("Clock Speed", "Speed at which the sequence will interpolate between frames", init, ClockIncrmentMag, ClockSpeedCap, ClockSpeedMin, SequenceSpeedIncrement);
            }

            public void ZeroSequence()
            {
                RisidualClockMode = CurrentClockMode;
                LoadKeyFrames(-1);
                CurrentClockMode = ClockMode.PAUSE;
                CurrentClockTime = 0;
            }
            public void SetClockMode(ClockMode mode)
            {
                RisidualClockMode = mode == ClockMode.PAUSE ? CurrentClockMode : mode;
                CurrentClockMode = mode;
            }
            public void ToggleClockPause()
            {
                CurrentClockMode = CurrentClockMode == ClockMode.PAUSE ? RisidualClockMode : ClockMode.PAUSE;
            }
            public void ToggleClockDirection()
            {
                RisidualClockMode = RisidualClockMode == ClockMode.FOR ? ClockMode.REV : ClockMode.FOR;
                CurrentClockMode = CurrentClockMode == ClockMode.PAUSE ? CurrentClockMode : RisidualClockMode;
            }

            public bool DemoKeyFrame(int index)
            {
                if (index < 0 ||
                    index >= Frames.Count)
                    return false;

                foreach (JointFrame jFrame in Frames[index].Jframes)
                    jFrame.Joint.OverwriteAnimTarget(jFrame.MySetting.MyValue());

                return true;
            }
            public bool UpdateSequence(bool anim)
            {
                if (CurrentFrames == null ||
                    CurrentClockMode == ClockMode.PAUSE)
                    return false;

                //StreamDlog("Update Sequence...");
                if (anim)
                    UpdateSequenceClock();
                else
                    UpdateTriggers();

                LerpFrame(CurrentClockTime);
                return true;
            }
            public bool AddKeyFrameSnapshot(int index, string name = null)//, bool snapping = false)
            {
                if (Parent == null ||
                    JointSet.Joints.Count == 0)
                    return false;

                if (name == null)
                    name = $"Frame_{index}";

                Frames.Insert(index, NewKeyFrame(JointSet, name));

                return true;
            }
            public bool RemoveKeyFrameAtIndex(int index)
            {
                if (index < 0 ||
                    index >= Frames.Count)
                    return false;

                Frames.RemoveAt(index);
                return true;
            }

            void UpdateTriggers()
            {
                if (WithinTargetThreshold)
                    UpdateSequenceClock();

                if (CheckFrameTimer())
                    LoadKeyFrames();

                UpdateStepDelay();

                if (!IgnoreFeet.MyState() && !StepDelay && JointSet.CheckStep())
                {

                    StepDelay = true;
                    JointSet.IncrementStepping(CurrentClockMode);
                    LoadKeyFrames();
                }
            }
            void UpdateSequenceClock()
            {
                CurrentClockTime += MySetting.MyValue() * (1 / CurrentFrames[0].MySetting.MyValue()) * (int)CurrentClockMode;
                CurrentClockTime = CurrentClockTime < 0 ? 0 : CurrentClockTime > 1 ? 1 : CurrentClockTime;
            }
            void UpdateStepDelay()
            {
                if (CurrentClockMode == ClockMode.PAUSE ||
                    !StepDelay)
                    return;

                float triggerTime = CurrentClockMode != ClockMode.REV ? CurrentClockTime : 1 - CurrentClockTime;

                if (triggerTime >= StepThreshold.MyValue())
                    StepDelay = false;
            }
            void LerpFrame(float lerpTime)
            {
                foreach (Joint joint in JointSet.Joints)
                {
                    if (!joint.IsAlive() || joint.OverrideSequenceID != UniqueID)
                        continue;

                    joint.LerpAnimationFrame(lerpTime);
                }
            }

            bool CheckFrameTimer()
            {
                if (!WithinTargetThreshold)
                    return false;
                if (CurrentClockMode == ClockMode.FOR && CurrentClockTime == 1)
                    return true;
                if (CurrentClockMode == ClockMode.REV && CurrentClockTime == 0)
                    return true;
                return false;
            }
            public bool LoadKeyFrames(int lockedFrameIndex = -1)
            {
                bool forward = CurrentClockMode != ClockMode.REV;
                CurrentClockTime = forward ? 0 : 1;

                int indexZero = CurrentFrames[0] == null || lockedFrameIndex != -1 ?
                    forward ? lockedFrameIndex : NextFrameIndex(lockedFrameIndex) :
                    NextFrameIndex(CurrentFrames[0].UniqueID);

                int indexOne = CurrentFrames[1] == null || lockedFrameIndex != -1 ?
                    forward ? NextFrameIndex(lockedFrameIndex) : lockedFrameIndex :
                    NextFrameIndex(CurrentFrames[1].UniqueID);

                CurrentFrames[0] = Frames[indexZero];
                CurrentFrames[1] = Frames[indexOne];

                return LoadJointFrames(forward, lockedFrameIndex != -1);
            }
            bool LoadJointFrames(bool forward = true, bool interrupt = false)
            {
                if (Parent == null)
                    return false;

                JointFrame zero, one;
                //Joint joint;

                //for (int i = 0; i < JointSet.Joints.Count; i++)
                foreach (Joint joint in JointSet.Joints)
                {
                    //joint = JointSet.GetJointByIndex(i);
                    if (joint is Piston || joint.OverrideSequenceID != UniqueID || !joint.IsAlive())
                        continue;

                    zero = CurrentFrames[0] == null ? null : CurrentFrames[0].GetChildByID<JointFrame>(joint.UniqueID);
                    one = CurrentFrames[1] == null ? null : CurrentFrames[1].GetChildByID<JointFrame>(joint.UniqueID);
                    joint.LoadJointFrames(zero, one, forward, interrupt);
                }
                return true;
            }

            int NextFrameIndex(int current)
            {
                int next = current + (CurrentClockMode != ClockMode.REV ? 1 : -1);
                next = next < 0 ? Frames.Count - 1 : next >= Frames.Count ? 0 : next;
                return next;
            }
        }

    }
}
