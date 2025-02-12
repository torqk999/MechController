//using Sandbox.ModAPI;

namespace IngameScript
{
    partial class Program
    {
        enum PARAM_custom
        {
            Name = 1,
            TAG = 0,

            uIX = 2, // unique index (my)
            pIX = 3, // parent
            fIX = 4, // foot
            lIX = 5, // lock
            sIX = 6, // sync

            SettingInit = 4,
            //GroupName = 4,
            GripDirection = 5,
        }

        enum PARAM_string {
            Name = 1,
            TAG = 0,
        }

        enum PARAM_int {
            uIX = 0, // unique index (my)
            pIX = 1, // parent
            fIX = 2, // foot
            lIX = 3, // lock
            sIX = 3, // sync
            GripDirection = 3
        }

    }
}
