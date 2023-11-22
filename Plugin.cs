using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using FlashUtils.Patches;
using HarmonyLib;

namespace FlashUtils;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);
    internal static Plugin Instance;

    public ConfigEntry<bool> ConfigInfiniteSprint { get; private set; }
    public ConfigEntry<int> ConfigDeadlineDays { get; private set; }
    public ConfigEntry<float> ConfigSprintTime { get; private set; }

    internal static ManualLogSource logger;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        logger = Logger;

        Logger.LogInfo("Loading Config...");
        ConfigInfiniteSprint = Config.Bind("Stamina", "InfiniteSprint", false, "Whether or not to enable infinite sprint.");
        ConfigSprintTime = Config.Bind("Stamina", "SprintTime", 5f, "Multiplier used to determine sprint cost.");

        ConfigDeadlineDays = Config.Bind("Quota", "DeadlineDays", 4, "How many days until the deadline.");

        Logger.LogInfo("Patching PlayerControllerB...");
        harmony.PatchAll(typeof(PlayerControllerBPatch));
        Logger.LogInfo("Patching TimeOfDay...");
        harmony.PatchAll(typeof(TimeOfDayPatch));

        Logger.LogInfo("Done!");
    }
}

