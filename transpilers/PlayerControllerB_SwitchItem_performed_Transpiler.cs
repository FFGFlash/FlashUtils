using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using GameNetcodeStuff;
using HarmonyLib;

namespace FlashUtils;

[HarmonyPatch(typeof(PlayerControllerB))]
[HarmonyPatch("SwitchItem_performed")]
public class PlayerControllerB_SwitchItem_performed_Transpiler
{
  static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
  {
    // Original Instructions:
    // IL_00C9: ldarga.s  context
    // IL_00CB: call      instance !!0 [Unity.InputSystem]UnityEngine.InputSystem.InputAction/CallbackContext::ReadValue<float32>()
    // IL_00D0: ldc.r4    0.0
    // IL_00D5: ble.un    IL_00EE
    // New Instructions:
    // IL_00C9: ldarga.s  context
    // IL_00CB: call      instance !!0 [Unity.InputSystem]UnityEngine.InputSystem.InputAction/CallbackContext::ReadValue<float32>()
    // IL_00D0: ldc.r4    0.0
    // IL_00D5: bge.un    IL_00EE

    var codes = new List<CodeInstruction>(instructions);

    var found = false;
    for (var i = codes.Count - 1; i >= 3; i--)
    {
      var contextArg = codes[i - 3];
      var call = codes[i - 2];
      var constant = codes[i - 1];
      var comparison = codes[i];
      if (
        contextArg.opcode != OpCodes.Ldarga_S ||
        call.opcode != OpCodes.Call || call.operand.ToString() != "Single ReadValue[Single]()" ||
        constant.opcode != OpCodes.Ldc_R4 || !constant.OperandIs(0f) ||
        comparison.opcode != OpCodes.Ble_Un
      )
        continue;
      var newComparison = new CodeInstruction(OpCodes.Bge_Un, comparison.operand);
      codes.RemoveAt(i);
      codes.Insert(i, newComparison);
      found = true;
    }

    if (found is false)
      Plugin.logger.LogInfo("Failed to initialize mouse inversion.");

    return codes.AsEnumerable();
  }
}
