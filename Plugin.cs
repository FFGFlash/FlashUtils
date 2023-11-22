using BepInEx;
using FlashUtils.Patches;
using HarmonyLib;

namespace FlashUtils
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);
        private static Plugin Instance;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            harmony.PatchAll(typeof(PlayerControllerBPatch));
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
