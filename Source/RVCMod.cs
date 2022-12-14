using System.IO;
using UnityEngine;
using Verse;

namespace RVCRestructured
{
    public class RVCSettings : ModSettings
    {

    }
    public class RimValiCore : Mod
    {
        public string DataPath
        {
            get
            {
                return $"{Application.dataPath}/../RimValiCore";
            }
        }

        private static string dir;
        public static string ModDir
        {
            get
            {
                return dir;
            }
        }

        public RimValiCore(ModContentPack content) : base(content)
        {
            dir = content.RootDir;
            if (!Directory.Exists(DataPath))
            {
                Log.Message("[RVC] Doing first time setup..");
                Directory.CreateDirectory(DataPath);
            }
            RVCLog.Log("Initalized mod.");
        }
    }
}