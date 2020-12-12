using Harmony;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;

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
				test();
			}
		}

		[HarmonyPatch(typeof(CueDartManager), "ShouldCreateDart")]
		class CueDartManager_ShouldCreateDart {
			static bool Prefix (ref bool __result) {
				var shouldDart = ShouldDart();
				if (Config.HiddenDarts == true || ForceEnable == true || Data.firstDart == true || shouldDart == true) {
					if (Data.firstDart == true || shouldDart == true) {
						Data.firstDart = false;
						__result = true;
					} else {
						__result = false;
					}
				}
				return __result;
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

		public static bool ShouldDart () {
			try {
				if (Data.darts.Length == 0) { return false; }
				int currTick = (int)AudioDriver.I.mCachedTick;
				if (currTick >= Data.darts[0][0] & currTick <= Data.darts[0][1]) {
					// MelonLogger.Log($"DARTING: {(int)AudioDriver.I.mCachedTick}");
					return true; 
				}
				if (currTick > Data.darts[0][1]) { Data.darts = Data.darts.Where((item, index) => index != 0).ToArray(); } // we dont talk about this

				return false;
			} catch (Exception e) {
				MelonLogger.LogError(e.ToString());
				return false;
			}
		}

		public static void test () {
			var candidates = GetQuietParts();
			List<CueRange> ranges2 = new List<CueRange>();

			candidates.ForEach((queData) => {
				var ms = AudioDriver.TickSpanToMs(SongDataHolder.I.songData, queData.fromTick, queData.toTick);
				if (ms >= 2000f) { ranges2.Add(queData); }
			});

			// MelonLogger.Log($"Found {ranges2.Count} candidates!");
			Data.darts = new int[ranges2.Count][];
			
			int i = 0;
			foreach (CueRange Cue in ranges2) {
				Data.darts[i] = new int[] { Cue.enableFrom, Cue.enableTo };
				i++;
			}
		}

		public static List<CueRange> GetQuietParts (int minDistance = 1920) {
			SongCues.Cue[] cues = SongCues.I.mCues.cues;
			List<CueRange> ranges = new List<CueRange>();
			int lastTick = 0;
			for (int i = 0; i < cues.Length; i++) {
				if (cues[i].behavior == Target.TargetBehavior.Melee || cues[i].behavior == Target.TargetBehavior.Dodge) { continue; }
					
				if (cues[i].tick - lastTick >= minDistance && lastTick != 0 && i > 2) {
					ranges.Add(new CueRange(lastTick, cues[i].tick, cues[i - 2].tick, cues[i].tick));
				}
				lastTick = cues[i].tick;
			}

			return ranges;
		}

		public struct CueRange {
			public int fromTick;
			public int toTick;
			public int enableFrom;
			public int enableTo;
			public CueRange (int _fromTick, int _toTick, int _enableFrom, int _enableTo) {
				fromTick = _fromTick;
				toTick = _toTick;
				enableFrom = _enableFrom;
				enableTo = _enableTo;
			}
		}
	}
}

