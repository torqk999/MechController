//using Sandbox.ModAPI;

using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    partial class Program
    {
        class Root {
            public virtual string Name {
                get { return "[ROOT]"; }
                set { Static("No name value to set!\n"); }
            }

            public string TAG = string.Empty;
            public int UniqueID;

            public Root Parent;

            public bool BUILT;
            static string[] SaveBuffer = new string[EnumLength(typeof(SaveDataAttribute))];
            public Root(string input)
            {
                BUILT = Load(input);
            }
            public Root(int uniqueID, string name = null)
            {
                Name = name;
                UniqueID = uniqueID;
                BUILT = true;
            }

            public virtual void AddChild<T>(T child) where T : Root { }
            public virtual void AddChildren<T>(List<T> children) where T : Root { }
            public virtual T GetChildByID<T>(int ID) where T : Root{ return null; }
            public virtual T GetChildByIndex<T>(int index) where T : Root{ return null; }


            public void StaticDlog(string input, bool newLine = true)
            {
                Static($"{input}{(newLine ? "\n" : "")}");
            }
            public void StreamDlog(string input, bool newLine = true)
            {
                DebugBinStream.Append($"{input}{(newLine ? "\n" : "")}");
            }

            protected bool Load(string input)
            {
                return Load(input.Split(':'));
            }
            protected virtual bool Load(string[] data)
            {
                try
                {
                    Name = data[(int)SaveDataAttribute.Name];
                    TAG = data[(int)SaveDataAttribute.TAG];
                    UniqueID = int.Parse(data[(int)SaveDataAttribute.UniqueID]);

                    return true;
                }
                catch { return false; }
            }
            //public string[] SaveDataArray()
            //{
            //    saveData(SaveBuffer);
            //    return SaveBuffer;
            //}
            public string SaveData()
            {
                saveData(SaveBuffer);

                RootDataBuilder.Clear();

                for (int i = 0; i < SaveBuffer.Length; i++)
                    RootDataBuilder.Append($"{SaveBuffer[i]}:");

                return RootDataBuilder.ToString();
            }
            protected virtual void saveData(string[] saveBuffer)
            {
                saveBuffer[(int)SaveDataAttribute.Name] = Name;
                saveBuffer[(int)SaveDataAttribute.TAG] = TAG;
                saveBuffer[(int)SaveDataAttribute.UniqueID] = UniqueID.ToString();
            }

        }

    }
}
