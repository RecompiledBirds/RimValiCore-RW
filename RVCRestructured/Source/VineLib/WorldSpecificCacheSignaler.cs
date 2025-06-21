using RimWorld.Planet;

namespace RVCRestructured;
public class WorldSpecificCacheSignaler(World world) : WorldComponent(world)
{
    public static Action? signalCachesReset;
    public override void FinalizeInit(bool fromLoad)
    {
        signalCachesReset?.Invoke();
        base.FinalizeInit(fromLoad);
    }
}
