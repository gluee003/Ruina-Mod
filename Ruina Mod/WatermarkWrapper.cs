using System.Runtime.CompilerServices;

namespace Ruina_Mod
{
    internal class WatermarkWrapper
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void ActivateWatermark() => AddWatermark.API.ActivateWatermark();
    }
}
