using HarmonyLib;
using GameNetcodeStuff;

namespace FlashUtils.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal class PlayerControllerBPatch
{
  [HarmonyPatch("Update")]
  [HarmonyPostfix]
  private static void patchUpdate(ref float ___sprintMeter) {
    ___sprintMeter = 1f;
  }
}
