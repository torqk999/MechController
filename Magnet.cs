//using Sandbox.ModAPI;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Linq;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {

        class Magnet : Functional
        {
            //public int FootIndex;
            public IMyLandingGear Gear;
            public Magnet(IMyLandingGear gear, int[] intData) : base(gear, intData)
            {
                TAG = MagnetTag;
                Gear = gear;
            }
            public Magnet(IMyLandingGear gear) : base(gear)
            {
                Gear = gear;
            }

            //public override int[] IntParams() {
            //    int[] result = base.IntParams();
            //
            //    result[(int)PARAM_int.fIX] = FootIndex;
            //
            //    return result;
            //}

            //public override List<int> Indexes()
            //{
            //    List<int> indexes = base.Indexes();
            //
            //    indexes.Add(FootIndex);
            //
            //    return indexes;
            //}

            protected override bool Load(string[] data)
            {
                if (!base.Load(data))
                    return false;

                try { FootIndex = int.Parse(data[(int)PARAM_custom.fIX]); }
                catch { FootIndex = -1; }
                return true;
            }

            protected override void saveData(string[] buffer)
            {
                base.saveData(buffer);
                buffer[(int)PARAM_custom.fIX] = FootIndex.ToString();
            }

            public void InitializeGear()
            {
                Gear.AutoLock = false;
                Gear.Enabled = true;
            }

            public void ToggleLock(bool locking)
            {
                Gear.AutoLock = locking;
                if (locking)
                {
                    Gear.Lock();
                    Gear.AutoLock = true;
                }
                else
                {
                    Gear.Unlock();
                    Gear.AutoLock = false;
                }

            }
            //public bool IsAlive()
            //{
            //    try { return Gear.IsWorking; }
            //    catch { return false; }
            //}
            public bool IsTouching()
            {
                return Gear.LockMode == LandingGearMode.ReadyToLock;
            }
            public bool IsLocked()
            {
                return Gear.LockMode == LandingGearMode.Locked;
            }
        }

    }
}
