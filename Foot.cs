using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        class Foot : Group {

            public List<Root> Toes = new List<Root>();
            public List<Root> Planars = new List<Root>();
            public List<Root> Magnets = new List<Root>();

            public bool Planeing;
            public Vector3 PlanarRatio;

            public override string Name {
                get { return $"[FOOT:{MyIndex}]"; }
            }

            public Foot(IMyBlockGroup group, int[] intData) : base(group, intData)
            {
                LockedIndex = intData[(int)PARAM_int.lIX];
            }

            public Foot(string input) : base(input)
            {
                //StaticDlog("Foot Constructor:");
                //BUILT = Load(input);
            }

            public Joint GetToe(int index)
            {
                if (index < 0 || index >= Toes.Count)
                    return null;
                return (Joint)Toes[index];
            }
            public Joint GetPlanar(int index)
            {
                if (index < 0 || index >= Planars.Count)
                    return null;
                return (Joint)Planars[index];
            }
            public Magnet GetMagnet(int index)
            {
                if (index < 0 || index >= Magnets.Count)
                    return null;
                return (Magnet)Magnets[index];
            }

            public void GearInit()
            {
                foreach (Magnet magnet in Magnets)
                    magnet.InitializeGear();
            }
            public void ToggleLock(bool locking = true)
            {
                foreach (Magnet magnet in Magnets)
                    magnet.ToggleLock(locking);

                Locked = locking;
                ToggleGrip(locking);
                ToggleForce(locking);
                UpdateFootPlaneing(Planeing);
            }

            // + - * / 

            public bool CheckTouching()
            {
                bool result = false;
                foreach (Magnet magnet in Magnets)
                    if (magnet.IsAlive() && magnet.IsTouching())
                    {
                        result = true;
                        break;
                    }

                return result;
            }
            public bool CheckLocked()
            {
                bool result = false;
                foreach (Magnet gear in Magnets)
                    if (gear.IsAlive() && gear.IsLocked())
                    {
                        result = true;
                        break;
                    }

                return result;
            }
            void ToggleGrip(bool gripping = true)
            {
                foreach (Joint toe in Toes)
                    toe.Gripping = gripping;
            }
            void ToggleForce(bool maxForce)
            {
                foreach (Joint planar in Planars)
                    planar.SetForce(maxForce);
            }
            public void UpdateFootPlaneing(bool toggle)
            {
                Planeing = toggle;

                foreach (Joint plane in Planars)
                    if (plane != null)
                    {
                        plane.Planeing = (Locked || (plane.TAG == TurnTag && Strafing.MyState())) && Planeing;
                    }
            }
            public void GenerateAxisMagnitudes(MatrixD plane)
            {
                PlanarRatio = Vector3.Zero;

                for (int i = 0; i < Planars.Count; i++)
                {
                    if (Planars[i] == null)
                        continue;

                    GetPlanar(i).UpdatePlanarDot(plane);
                    for (int j = 0; j < 3; j++)
                    {
                        PlanarRatio.SetDim(j, PlanarRatio.GetDim(j) + Math.Abs(GetPlanar(i).PlanarDots.GetDim(j)));
                    }
                }

                for (int i = 0; i < 3; i++)
                    PlanarRatio.SetDim(i, 1 / PlanarRatio.GetDim(i));
            }

            //protected override bool Load(string[] data)
            //{
            //    if (!base.Load(data))
            //        return false;
            //
            //    try { LockIndex = int.Parse(data[(int)PARAM_custom.lIX]); }
            //    catch { LockIndex = -1; }
            //    return true;
            //}

            //protected override void saveData(string[] buffer)
            //{
            //    base.saveData(buffer);
            //    buffer[(int)PARAM_custom.lIX] = LockIndex.ToString();
            //}
        }

    }
}
