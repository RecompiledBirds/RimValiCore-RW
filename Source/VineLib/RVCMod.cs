using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Verse;

namespace RVCRestructured
{
    public class RVCSettings : ModSettings
    {

        public override void ExposeData()
        {
            base.ExposeData();
        }
    }

    public class RimValiCore : Mod
    {
        public static RVCSettings settings;
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
            settings=GetSettings<RVCSettings>();
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