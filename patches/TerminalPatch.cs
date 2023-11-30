using HarmonyLib;

namespace FlashUtils;

[HarmonyPatch(typeof(Terminal))]
internal class TerminalPatch
{
  [HarmonyPatch("Update")]
  [HarmonyPrefix]
  private static void Update(ref int ___groupCredits)
  {
    if (Plugin.Instance.ConfigInfiniteCredits.Value) ___groupCredits = 999999;
  }
}
