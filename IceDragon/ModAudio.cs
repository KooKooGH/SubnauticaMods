using Nautilus.FMod;
using Nautilus.Utility;

namespace IceDragon;

public static class ModAudio
{
    public static void RegisterAudio()
    {
        var builder = new FModSoundBuilder(new AssetBundleSoundSource(Plugin.Bundle));
        
        builder.CreateNewEvent("FrozenIceDragonFlail", AudioUtils.BusPaths.SFX)
            .SetMode3D(8, 50)
            .SetSound("FrozenIceDragonTailLoop")
            .Register();
        
        builder.CreateNewEvent("FrozenIceDragonRoarOnScan", AudioUtils.BusPaths.SFX)
            .SetMode3D(8, 60)
            .SetSound("FrozenIceDragonRoarHallucinationCin")
            .Register();
    }
}