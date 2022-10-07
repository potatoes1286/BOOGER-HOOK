using System;
using BepInEx;
using FistVR;
using HarmonyLib;
using UnityEngine;

namespace H3VRMod
{
	[BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
	[BepInProcess("h3vr.exe")]
	public class Plugin : BaseUnityPlugin
	{

		public static int booger_hookd = 0;

		private void Awake()
		{
			Harmony.CreateAndPatchAll(typeof(Plugin));
		}

		[HarmonyPatch(typeof(FVRInteractiveObject), "BeginInteraction")]
		[HarmonyPostfix]
		public static void BOOGER_HOOK(FVRInteractiveObject __instance)
		{
			if (__instance.m_hand.Trigger_Touch.state)
			{
				if (__instance is FVRFireArm)
				{
					if (__instance is ClosedBoltWeapon)
					{
						var firearm = __instance as ClosedBoltWeapon;
						ClosedBoltWeapon.FireSelectorModeType modeType = firearm.FireSelector_Modes[firearm.m_fireSelectorMode].ModeType;
						if (firearm.Bolt.CurPos == ClosedBolt.BoltPos.Forward && modeType != ClosedBoltWeapon.FireSelectorModeType.Safe)
						{
							firearm.DropHammer();
							firearm.m_hasTriggerReset = false;
						}
					}

					if (__instance is OpenBoltReceiver)
					{
						var firearm = __instance as OpenBoltReceiver;
						if (firearm.HasTriggerButton&&
						    firearm.FireSelector_Modes[firearm.m_fireSelectorMode].ModeType != 0)
						{
							if (firearm.Bolt.CurPos == OpenBoltReceiverBolt.BoltPos.Forward)
								return;
							firearm.ReleaseSeer();
						}
					}

					if (__instance is Handgun)
					{
						var firearm = __instance as Handgun;
						if(!firearm.IsSafetyEngaged)
							firearm.ReleaseSeer();
					}

					if (__instance is BoltActionRifle)
					{
						var firearm = __instance as BoltActionRifle;
						if (firearm.FireSelector_Modes[firearm.m_fireSelectorMode].ModeType != 0
						    && firearm.BoltHandle.HandleState == BoltActionRifle_Handle.BoltActionHandleState.Forward
						    && firearm.BoltHandle.HandleRot != 0)
						{
							firearm.DropHammer();
						}
					}
				}
			}
		}
		//public SteamVR_Action_Boolean Trigger_Touch; FVRViveHand
	}
}