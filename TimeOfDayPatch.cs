using HarmonyLib;

namespace FlashUtils.Patches;

[HarmonyPatch(typeof(TimeOfDay))]
internal class TimeOfDayPatch
{
  [HarmonyPatch("Awake")]
  [HarmonyPostfix]
  private static void PatchAwake(ref QuotaSettings ___quotaVariables)
  {
    Plugin.logger.LogInfo("We awake?");
    ___quotaVariables.deadlineDaysAmount = Plugin.Instance.ConfigDeadlineDays.Value;
  }
}
