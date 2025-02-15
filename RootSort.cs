//using Sandbox.ModAPI;
using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        class RootSort : IComparer<Root>
        {
            public int Compare(Root x, Root y)
            {
                if (x != null && y != null)
                    return x.UniqueID.CompareTo(y.UniqueID);
                else
                    return 0;
            }
        }

    }
}
