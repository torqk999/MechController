//using Sandbox.ModAPI;
using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {


        class JointSet : FunctionalGroup
        {

            public IMyTerminalBlock Plane;
            public List<Foot> Feet = new List<Foot>();
            public List<Joint> Joints = new List<Joint>();
            public List<Sequence> Sequences = new List<Sequence>();

            public KeyFrame ZeroFrame = null;

            public MatrixD TargetPlane;
            public MatrixD TurnPlane;
            public MatrixD BufferPlane;
            public Vector3D PlaneBuffer;
            public Vector3D TurnBuffer;

            //public bool Locked;
            public bool StepInterrupt;
            //public int LockedFootID;
            public int LockedFootIndex;
            int ReleaseTimer = 0;

            public JointSet(IMyTerminalBlock plane, IMyBlockGroup group, int uniqueID) : base(group, uniqueID)
            {
                TAG = JointSetTag;
                Plane = plane;
                GenerateZeroFrame();
            }

            public JointSet(string input, IMyTerminalBlock plane, List<Foot> buffer) : base(input)
            {
                Plane = plane;
                Feet.AddRange(buffer);
            }

            public void GenerateZeroFrame() { ZeroFrame = NewKeyFrame(this, "Zero Frame"); }

            public override void AddChild<T>(T child) {
                if (child == null) return;

                if (typeof(T) == typeof(Joint))
                    Joints.Add(child as Joint);
                else if (typeof(T) == typeof(Foot))
                    Feet.Add(child as Foot);
                else if (typeof(T) == typeof(Sequence))
                    Sequences.Add(child as Sequence);
                else return;

                child.Parent = this;
            }

            public override T GetChildByID<T>(int ID) {
                if (typeof(T) == typeof(Joint))
                    return Joints.Find(x => x.UniqueID == ID) as T;
                if (typeof(T) == typeof(Foot))
                    return Feet.Find(x => x.UniqueID == ID) as T;
                if (typeof(T) == typeof(Sequence))
                    return Sequences.Find(x => x.UniqueID == ID) as T;
                return null;
            }

            public override T GetChildByIndex<T>(int index) {
                if (index < 0)
                    return null;
                if (typeof(T) == typeof(Joint))
                    return index >= Joints.Count ? null : Joints[index] as T;
                if (typeof(T) == typeof(Foot))
                    return index >= Feet.Count ? null : Feet[index] as T;
                if (typeof(T) == typeof(Sequence))
                    return index >= Sequences.Count ? null : Sequences[index] as T;
                return null;
            }

            public bool UpdateJoints()
            {
                bool withinThreshold = true;

                foreach (Joint joint in Joints)
                {
                    if (!joint.IsAlive())
                        continue;

                    joint.UpdateJoint(StatorTarget.MyState());
                    if (withinThreshold)
                        withinThreshold = joint.TargetThreshold;
                }

                return withinThreshold;
            }
            public bool UpdateFootLockStatus()
            {
                ReleaseTimer -= 1;
                ReleaseTimer = ReleaseTimer < 0 ? 0 : ReleaseTimer;

                if (StepInterrupt)
                    return false;

                bool oldState = Locked;
                Foot locked = GetChildByIndex<Foot>(LockedFootIndex);
                Locked = locked != null && locked.CheckLocked();


                if (!Locked && ReleaseTimer <= 0) // TouchDown
                    NewLockCandidate();

                UnlockOtherFeet();

                bool changed = oldState != Locked;

                return changed;
            }
            public bool CheckStep(bool forward = true)
            {
                Foot step = GetChildByIndex<Foot>(StepIndex(forward));
                Foot release = GetChildByIndex<Foot>(LockedFootIndex);

                bool touch = step != null && step.CheckTouching();
                bool locked = step != null && step.CheckLocked();
                bool released = release != null && !release.CheckLocked();

                if (!touch && !locked) // Any attempt to step?
                {
                    StepInterrupt = false;
                    return false;
                }

                StepInterrupt = true;

                if (touch) // Initial contact
                    step.ToggleLock(); // lock foot

                if (locked && release != null) // Initial lock
                    release.ToggleLock(false); // Release

                if (StepInterrupt && released) // Initial release
                    StepInterrupt = false;

                return !StepInterrupt; // Still Stepping?
            }
            public void UnlockAllFeet()
            {
                ReleaseTimer = ReleaseCount;
                LockedFootIndex = -1;
                UnlockOtherFeet();
            }
            public void IncrementStepping(ClockMode mode)
            {
                IncrementStepping(mode != ClockMode.REV ? 1 : -1);
            }
            public void InitFootStatus()
            {
                NewLockCandidate();
                UnlockOtherFeet();

                foreach (Foot foot in Feet)
                    foot.GearInit();
            }

            int StepIndex(bool forward)
            {
                int stepIndex = LockedFootIndex + (forward ? 1 : -1);
                stepIndex = stepIndex < 0 ? Feet.Count - 1 : stepIndex >= Feet.Count ? 0 : stepIndex;
                return stepIndex;
            }
            void UnlockOtherFeet()
            {
                Foot expected = GetChildByIndex<Foot>(LockedFootIndex);
                foreach (Foot foot in Feet)
                    if (foot != expected)
                        foot.ToggleLock(false);
            }
            void IncrementStepping(int incr)
            {
                SetLockedIndex(LockedFootIndex + incr);
            }
            void SetLockedIndex(int step)
            {
                LockedFootIndex = step;
                LockedFootIndex = LockedFootIndex < 0 ? Feet.Count - 1 : LockedFootIndex >= Feet.Count ? 0 : LockedFootIndex;
            }
            void NewLockCandidate()
            {
                for (int i = 0; i < Feet.Count; i++)
                {
                    Foot check = GetChildByIndex<Foot>(i);
                    if (check.CheckLocked() || check.CheckTouching())
                    {
                        check.ToggleLock(true);
                        LockedFootIndex = i;
                        Locked = true;

                        if (CurrentWalk != null)
                        {
                            CurrentWalk.LoadKeyFrames(check.LockFrameID);
                            CurrentWalk.StepDelay = true;
                        }


                        return;
                    }
                }

                Locked = false;
                LockedFootIndex = -1;
            }

            public void SyncJoints()
            {
                foreach (Joint joint in Joints)
                    joint.Sync();
            }
            public void ZeroJointSet(KeyFrame frame = null)
            {
                if (frame == null)
                    foreach (Joint joint in Joints)
                        joint.OverwriteAnimTarget(0);
                else
                    foreach (JointFrame jFrame in frame.Jframes)
                        jFrame.Joint.OverwriteAnimTarget(jFrame.MySetting.MyValue());
            }
            public void SnapShotPlane()
            {
                if (Plane == null)
                    return;

                TargetPlane = Plane.WorldMatrix;
                TurnPlane = Plane.WorldMatrix;
            }
            public void TogglePlaneing(bool toggle)
            {
                if (toggle && Locked)
                    SnapShotPlane();

                foreach (Foot foot in Feet)
                    foot.UpdateFootPlaneing(toggle && Locked);
            }
            void UpdatePlaneBuffer(Vector3 playerInput)
            {
                playerInput *= PlaneScalar;

                BufferPlane = MatrixD.CreateFromYawPitchRoll(playerInput.Y, playerInput.X, playerInput.Z);

                TargetPlane = MatrixD.Multiply(BufferPlane, TargetPlane);

                BufferPlane = MatrixD.Multiply(TargetPlane, MatrixD.Invert(Plane.WorldMatrix));

                MatrixD.GetEulerAnglesXYZ(ref BufferPlane, out PlaneBuffer);
            }
            void UpdateTurnBuffer(double playerTurn)
            {
                TurnBuffer.Y = playerTurn * TurnScalar;
            }
            public bool UpdatePlanars()
            {
                if (Plane == null)
                    return false;

                UpdatePlaneBuffer(InputRotationBuffer);
                UpdateTurnBuffer(InputTurnBuffer);

                bool safety = false;
                for (int i = 0; i < 3; i++)
                    if (Math.Abs(PlaneBuffer.GetDim(i)) > SAFETY)
                    {
                        SnapShotPlane();
                        break;
                    }

                foreach (Foot foot in Feet)
                {
                    if (foot != null)
                    {
                        foot.GenerateAxisMagnitudes(Plane.WorldMatrix);
                        for (int i = 0; i < foot.Planars.Count; i++)
                            if (foot.Planars[i] != null)
                            {
                                Joint plane = foot.GetPlanar(i);

                                if (safety)
                                {
                                    plane.PlaneCorrection = 0;
                                    continue;
                                }

                                if (plane.TAG == TurnTag && !foot.Locked)
                                {
                                    plane.PlaneCorrection = GeneratePlaneCorrection(plane, foot.PlanarRatio, TurnBuffer);
                                    DebugBinStream.Append($"PlaneCorrection Lifted {i}: {plane.PlaneCorrection}\n");
                                }

                                else
                                {
                                    plane.PlaneCorrection = GeneratePlaneCorrection(plane, foot.PlanarRatio, -PlaneBuffer);
                                }
                            }
                    }
                }
                return true;
            }
            double GeneratePlaneCorrection(Joint joint, Vector3 planarRatios, Vector3 angleCorrections)
            {
                if (joint == null)
                    return 0;

                double output = 0;
                for (int i = 0; i < 3; i++)
                {
                    double planarsum = joint.PlanarDots.GetDim(i) * planarRatios.GetDim(i) * (angleCorrections.GetDim(i) * RAD2DEG);

                    /*
                    
                     1,0,0
                     0,1,0
                     0,0,-1

                     */

                    // x = + / y = + / z = -
                    //output = i == 2 ? output - planarsum : output + planarsum;
                    output += planarsum;
                }
                return output;
            }
        }


    }
}
