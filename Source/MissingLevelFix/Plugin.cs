using BepInEx;
using HarmonyLib;
using MissingLevelFix.Properties;

namespace MissingLevelFix
{
    [BepInPlugin(ModInfo.ID, ModInfo.NAME, ModInfo.VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Harmony harmony = new Harmony(ModInfo.ID);
            harmony.PatchAll();
        }
    }
}
