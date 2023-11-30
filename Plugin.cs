using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using FlashUtils.Patches;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace FlashUtils;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private readonly Harmony harmony = new(PluginInfo.PLUGIN_GUID);
    internal static Plugin Instance;

    public static readonly ActionItem[] actions = new ActionItem[] {
        new("Emote1", Key.Z, "Dance Emote", 1),
        new("Emote2", Key.X, "Point Emote", 2),
        new("Slot1", Key.Digit1, "Equip Slot 1", 0),
        new("Slot2", Key.Digit2, "Equip Slot 2", 1),
        new("Slot3", Key.Digit3, "Equip Slot 3", 2),
        new("Slot4", Key.Digit4, "Equip Slot 4", 3),
    };

    public ConfigEntry<bool> ConfigEnabled { get; private set; }
    public ConfigEntry<bool> ConfigInvertScroll { get; private set; }
    public ConfigEntry<bool> ConfigDisableBoot { get; private set; }
    public ConfigEntry<bool> ConfigInfiniteSprint { get; private set; }
    public ConfigEntry<bool> ConfigInfiniteCredits { get; private set; }
    public ConfigEntry<int> ConfigStartingCredits { get; private set; }
    public ConfigEntry<bool> ConfigWeightless { get; private set; }
    public ConfigEntry<int> ConfigDeadlineDays { get; private set; }
    public ConfigEntry<float> ConfigSprintTime { get; private set; }

    internal static ManualLogSource logger;

    private void Awake()
    {
        Instance ??= this;
        logger ??= Logger;

        Logger.LogInfo("Loading Config...");
        ConfigEnabled = Config.Bind("General", "Enabled", true, "Whether the mod is enabled or not.");
        ConfigInvertScroll = Config.Bind("General", "InvertScroll", true, "Whether or not to invert the scroll wheel when switching items.");
        ConfigDisableBoot = Config.Bind("General", "DisableBootupScreen", false, "Whether or not to skip the boot up screen.");

        ConfigInfiniteCredits = Config.Bind("Credits", "InfiniteCredits", false, "Whether or not to have infinite credits.");
        ConfigStartingCredits = Config.Bind("Credits", "StartingCredits", 60, "How many credits to start with.");

        foreach (ActionItem item in actions)
            item.ConfigEntry = Config.Bind("Bindings", item.Id, item.Shortcut, item.Description);

        ConfigInfiniteSprint = Config.Bind("Stamina", "InfiniteSprint", false, "Whether or not to enable infinite sprint.");
        ConfigSprintTime = Config.Bind("Stamina", "SprintTime", 5f, "Multiplier used to determine sprint cost.");
        ConfigWeightless = Config.Bind("Stamina", "Weightless", false, "Remove all weight from the game.");

        ConfigDeadlineDays = Config.Bind("Quota", "DeadlineDays", 3, "How many days until the deadline.");

        if (ConfigEnabled.Value)
        {
            if (ConfigInvertScroll.Value)
            {
                Logger.LogInfo("Transpiling PlayerControllerB.SwitchItem_performed");
                harmony.PatchAll(typeof(PlayerControllerB_SwitchItem_performed_Transpiler));
            }
            Logger.LogInfo("Patching PlayerControllerB...");
            harmony.PatchAll(typeof(PlayerControllerBPatch));
            Logger.LogInfo("Patching TimeOfDay...");
            harmony.PatchAll(typeof(TimeOfDayPatch));
        }

        Logger.LogInfo("Done!");
    }
}

public class ActionItem
{
    public string Id { get; private set; }
    public Key Shortcut { get; private set; }
    public string Description { get; private set; }
    public int Value { get; private set; }
    public ConfigEntry<Key> ConfigEntry { get; set; }

    public ActionItem(string id, Key shortcut, string description, int value)
    {
        this.Id = id;
        this.Shortcut = shortcut;
        this.Description = description;
        this.Value = value;
    }
}