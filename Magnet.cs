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
            public IMyLandingGear Gear => (IMyLandingGear)FuncBlock;
            public Magnet(IMyLandingGear gear, int uniqueID, int footID = -1) : base(gear, uniqueID, footID) { }
            public Magnet(IMyLandingGear gear) : base(gear) { }

            protected override bool Load(string[] data)
            {
                if (!base.Load(data))
                    return false;

                try { FootID = int.Parse(data[(int)SaveDataAttribute.FootID]); }
                catch { FootID = -1; }
                return true;
            }

            protected override void saveData(string[] buffer)
            {
                base.saveData(buffer);
                buffer[(int)SaveDataAttribute.FootID] = FootID.ToString();
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
