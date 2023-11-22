using BepInEx;
using BepInEx.Configuration;
using FlashUtils.Patches;
using HarmonyLib;

namespace FlashUtils;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);
    internal static Plugin Instance;

    public ConfigEntry<bool> configInfiniteSprint { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;

        configInfiniteSprint = Config.Bind("Cheats", "InfiniteSprint", false, "Whether or not to enable infinite sprint.");

        harmony.PatchAll(typeof(PlayerControllerBPatch));
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }
}

