using HarmonyLib;

namespace Ruina_Mod
{
    public static class PInfo
    {
        // each loaded plugin needs to have a unique GUID. usually author+generalCategory+Name is good enough
        public const string GUID = "gluee.lbol.test.ruina";
        public const string Name = "Ruina Mod";
        public const string version = "0.0.1";
        public static readonly Harmony harmony = new Harmony(GUID);

    }
}
