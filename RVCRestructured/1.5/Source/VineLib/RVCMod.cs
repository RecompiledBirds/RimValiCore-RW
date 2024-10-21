using Verse;

namespace RVCRestructured;

public class RVCSettings : ModSettings
{
    public override void ExposeData()
    {
        base.ExposeData();
    }
}

public class RimValiCore : Mod
{
    private static RVCSettings? settings;

    public string DataPath => $"{Content.RootDir}/../../RimValiCore";

    private static string dir = string.Empty;
    public static string ModDir => dir;

    public static RVCSettings Settings 
    { 
        get => settings ?? throw new NullReferenceException(); 
        private set => settings = value; 
    }

    public RimValiCore(ModContentPack content) : base(content)
    {
        Settings = GetSettings<RVCSettings>();
        dir = content.RootDir;
        if (!Directory.Exists(DataPath))
        {
            Log.Message("[RVC] Doing first time setup..");
            Directory.CreateDirectory(DataPath);
        }

        RVCLog.Log("Initalized mod.");
    }
}
