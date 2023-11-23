using HarmonyLib;
using GameNetcodeStuff;

namespace FlashUtils.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal class PlayerControllerBPatch
{
  [HarmonyPatch("Awake")]
  [HarmonyPostfix]
  private static void PatchAwake(ref float ___sprintTime)
  {
    ___sprintTime = Plugin.Instance.ConfigSprintTime.Value;
  }

  [HarmonyPatch("Update")]
  [HarmonyPostfix]
  private static void PatchUpdate(ref float ___sprintMeter, ref float ___carryWeight)
  {
    if (Plugin.Instance.ConfigInfiniteSprint.Value) ___sprintMeter = 1f;
    if (Plugin.Instance.ConfigWeightless.Value) ___carryWeight = 1f;
  }
}
