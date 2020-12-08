using Harmony;
using MelonLoader;
using System;

namespace TimingAttack {
	public class TimingAttackClass: MelonMod {
		public static bool ForceEnable = false;

		public override void OnApplicationStart () {
			Config.RegisterConfig();
		}

		public override void OnModSettingsApplied () {
			Config.OnModSettingsApplied();
		}

		[HarmonyPatch(typeof(AudioDriver), "StartPlaying")]
		class AudioDriver_StartPlaying {
			static void Prefix () {
				Data.firstDart = true;
			}
		}

		[HarmonyPatch(typeof(CueDartManager), "ShouldCreateDart")]
		class CueDartManager_ShouldCreateDart {
			static void Postfix (ref bool __result) {
				if (Config.HiddenDarts == true || ForceEnable == true || Data.firstDart == true) {
					if (Data.firstDart == true) {
						Data.firstDart = false;
						__result = true;
					} else {
						__result = false;
					}
				}
			}
		}

		[HarmonyPatch(typeof(Telegraph), "Init", new Type[] { typeof(SongCues.Cue), typeof(float) })]
		private static class PatchInit {
			private static void Postfix (Telegraph __instance, SongCues.Cue cue, float animationSpeed) {
				if (Config.HiddenClouds == true || ForceEnable == true) {
					if (cue.behavior == Target.TargetBehavior.Melee || cue.behavior == Target.TargetBehavior.Dodge) { return; }
					__instance.cloud.enabled = false;
				}
			}
		}
	}
}

