using HarmonyLib;
using GameNetcodeStuff;

namespace FlashUtils.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal class PlayerControllerBPatch
{
  [HarmonyPatch("Update")]
  [HarmonyPostfix]
  private static void PatchUpdate(ref float ___sprintMeter)
  {
    if (Plugin.Instance.ConfigInfiniteSprint.Value) ___sprintMeter = 1f;
  }
}
