using HarmonyLib;

namespace FlashUtils.Patches;

[HarmonyPatch(typeof(TimeOfDay))]
internal class TimeOfDayPatch
{
  [HarmonyPatch("Awake")]
  [HarmonyPostfix]
  private static void PatchAwake(ref QuotaSettings ___quotaVariables)
  {
    ___quotaVariables.deadlineDaysAmount = Plugin.Instance.ConfigDeadlineDays.Value;
    ___quotaVariables.startingCredits = Plugin.Instance.ConfigStartingCredits.Value;
  }
}
