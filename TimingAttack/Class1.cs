using Harmony;
using MelonLoader;
using System;

namespace TimingAttack
{
    public class TimingAttackClass : MelonMod
    {
        public static bool ForceEnable = false;

        public override void OnApplicationStart() {
            Config.RegisterConfig();
        }

        public override void OnModSettingsApplied() {
            Config.OnModSettingsApplied();
        }

        [HarmonyPatch(typeof(CueDartManager), "ShouldCreateDart")]
        class CueDartManager_ShouldCreateDart {
            static void Postfix(ref bool __result) {
                if (Config.HiddenDarts == true || ForceEnable == true) { __result = false; }
            }
        }

        [HarmonyPatch(typeof(CueDart), "SetShowing")]
        class CueDart_SetShowing {
            static void Postfix(ref bool showing) {
                if (Config.HiddenDarts == true || ForceEnable == true) { showing = false; }
            }
        }

        [HarmonyPatch(typeof(Telegraph), "Init", new Type[] { typeof(SongCues.Cue), typeof(float) })]
        private static class PatchInit {
            private static void Postfix(Telegraph __instance, SongCues.Cue cue, float animationSpeed) {
                if (Config.HiddenClouds == true || ForceEnable == true) {
                    if (cue.behavior == Target.TargetBehavior.Melee || cue.behavior == Target.TargetBehavior.Dodge) { return; } 
                    // __instance.circleMesh.enabled = false;
                    __instance.cloud.enabled = false;
                }
            }
        }
    }
}

