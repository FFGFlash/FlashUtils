using HarmonyLib;
using GameNetcodeStuff;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine.InputSystem;
using UnityEngine;

namespace FlashUtils.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal class PlayerControllerBPatch
{
  private static readonly Dictionary<string, MethodInfo> MethodCache = new();
  private static readonly object[] ForwardParam = new object[] { true };
  private static readonly object[] BackwardParam = new object[] { false };

  [HarmonyPatch("Awake")]
  [HarmonyPostfix]
  private static void PatchAwake(ref float ___sprintTime, ref PlayerActions ___playerActions)
  {
    ___sprintTime = Plugin.Instance.ConfigSprintTime.Value;
  }

  [HarmonyPatch("Update")]
  [HarmonyPostfix]
  private static void PatchUpdate(PlayerControllerB __instance, ref float ___sprintMeter, ref float ___carryWeight, ref float ___timeSinceSwitchingSlots, ref bool ___throwingObject)
  {
    if ((!__instance.IsOwner || !__instance.isPlayerControlled || (__instance.IsServer && !__instance.isHostPlayerObject)) && !__instance.isTestingPlayer)
      return;

    if (Plugin.Instance.ConfigInfiniteSprint.Value) ___sprintMeter = 1f;
    if (Plugin.Instance.ConfigWeightless.Value) ___carryWeight = 1f;

    ActionItem action = Array.Find(Plugin.actions, (ActionItem item) => Keyboard.current[item.ConfigEntry.Value].wasPressedThisFrame);
    if (action != null)
    {
      switch (action.Id)
      {
        case "Emote1":
        case "Emote2":
          PerformEmote(__instance, action.Value);
          break;
        case "Slot1":
        case "Slot2":
        case "Slot3":
        case "Slot4":
          StopEmote(__instance);
          if (SwitchToSlot(__instance, action.Value, ___timeSinceSwitchingSlots, ___throwingObject))
            ___timeSinceSwitchingSlots = 0f;
          break;
      }
    }
  }

  private static object InvokePrivateMethod(PlayerControllerB instance, string methodName, object[] parameters = null)
  {
    MethodCache.TryGetValue(methodName, out var value);
    value ??= typeof(PlayerControllerB).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
    MethodCache[methodName] = value;
    return value?.Invoke(instance, parameters);
  }

  private static void PerformEmote(PlayerControllerB instance, int emoteId)
  {
    instance.timeSinceStartingEmote = 0f;
    instance.performingEmote = true;
    instance.playerBodyAnimator.SetInteger("emoteNumber", emoteId);
    instance.StartPerformingEmoteServerRpc();
  }

  private static void StopEmote(PlayerControllerB instance)
  {
    instance.performingEmote = false;
    instance.StopPerformingEmoteServerRpc();
    instance.timeSinceStartingEmote = 0f;
  }

  private static bool SwitchToSlot(PlayerControllerB instance, int requestedSlot, float timeSinceSwitchingSlots, bool isThrowingObject)
  {
    if (!CanSwitchSlot(instance, timeSinceSwitchingSlots, isThrowingObject) || instance.currentItemSlot == requestedSlot)
      return false;
    int num = instance.currentItemSlot - requestedSlot;
    bool flag = num > 0;
    if (Math.Abs(num) == instance.ItemSlots.Length - 1)
    {
      object[] parameters = flag ? ForwardParam : BackwardParam;
      InvokePrivateMethod(instance, "SwitchItemSlotsServerRPC", parameters);
    }
    else
    {
      object[] parameters = flag ? BackwardParam : ForwardParam;
      do
      {
        InvokePrivateMethod(instance, "SwitchItemSlotsServerRPC", parameters);
        num += flag ? -1 : 1;
      } while (num != 0);
    }
    ShipBuildModeManager.Instance.CancelBuildMode(true);
    instance.playerBodyAnimator.SetBool("GrabValidated", false);
    InvokePrivateMethod(instance, "SwitchToItemSlot", new object[] { requestedSlot, null });
    instance.currentlyHeldObjectServer?.gameObject.GetComponent<AudioSource>().PlayOneShot(instance.currentlyHeldObjectServer.itemProperties.grabSFX, 0.6f);
    return true;
  }

  private static bool CanSwitchSlot(PlayerControllerB instance, float timeSinceSwitchingSlots, bool isThrowingObject)
  {
    return !((double)timeSinceSwitchingSlots < 0.01 || instance.inTerminalMenu || instance.isGrabbingObjectAnimation || instance.inSpecialInteractAnimation || isThrowingObject) && !instance.isTypingChat && !instance.twoHanded && !instance.activatingItem && !instance.jetpackControls && !instance.disablingJetpackControls;
  }
}
