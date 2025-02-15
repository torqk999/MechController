//using Sandbox.ModAPI;

using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
    {
        class Animation : Root
        {
            protected string _name;
            public override string Name {
                get { return _name; }
                set { _name = value; }
            }
            public Setting MySetting;

            public Animation(float init, int uniqueID, string name = null) : base(uniqueID, name)
            {
                GenerateSetting(init);
            }

            public Animation(string input) : base(input)
            {
          
            }

            public virtual void GenerateSetting(float init)
            {

            }
            protected override bool Load(string[] data)
            {
                if (!base.Load(data))
                    return false;

                //try {
                    GenerateSetting(float.Parse(data[(int)SaveDataAttribute.SettingInit]));
                //}
                //catch {
                    //GenerateSetting(0);
                //}

                return true;
            }
            protected override void saveData(string[] buffer)
            {
                base.saveData(buffer);
                buffer[(int)SaveDataAttribute.SettingInit] = (MySetting == null ? 0 : MySetting.MyValue()).ToString();
            }
        }

    }
}
