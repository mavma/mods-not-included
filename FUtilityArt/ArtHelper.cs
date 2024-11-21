﻿using Database;
using FUtility;
using FUtilityArt.Components;
using System.Collections.Generic;

namespace FUtilityArt
{
	public class ArtHelper
	{
		public const string READY = "AwaitingArting";
		public const string UGLY = "LookingUgly";
		public const string OKAY = "LookingOkay";
		public const string GREAT = "LookingGreat";

		public static void GetDefaultDecors(ArtableStages artableStages, string id, out int ugly, out int mediocre, out int great, int fallbackUgly = 5, int fallbackMediocre = 10, int fallbackGreat = 15)
		{
			var stages = artableStages.GetPrefabStages(id);

			ugly = GetDefaultDecor(GREAT, stages, fallbackUgly);
			mediocre = GetDefaultDecor(OKAY, stages, fallbackMediocre);
			great = GetDefaultDecor(UGLY, stages, fallbackGreat);
		}

		private static int GetDefaultDecor(string status, List<ArtableStage> stages, int defaultIfNotFound)
		{
			// in case some other mod is trying to override these values, read it from their existing values
			var stage = stages.Find(s => s.statusItem?.Id == status);
			return stage == null ? defaultIfNotFound : stage.decor;
		}

		public static void MoveStages(List<ArtableStage> stages, Dictionary<string, string> targetStates, int uglyDecor, int okayDecor, int greatDecor)
		{
			if (stages == null)
			{
				Log.Warning("Invalid artable.");
			}

			if (targetStates == null || targetStates.Count == 0)
			{
				Log.Debug("no targetstates");
				return;
			}

			var idLookup = new Dictionary<string, string>()
			{
				{ "Bad", "MarbleSculpture_Bad" },
				{ "Average", "MarbleSculpture_Average" },
				{ "Good1", "MarbleSculpture_Good1" },
				{ "Good2", "MarbleSculpture_Good2" },
				{ "Good3", "MarbleSculpture_Good3" },
			};

			var mappedTargetStates = new Dictionary<string, ArtableStatusItem>();

			// convert to new naming scheme
			foreach (var state in targetStates)
			{
				var id = idLookup.TryGetValue(state.Key, out var newKey) ? newKey : state.Key;
				mappedTargetStates[id] = GetStatusItem(state.Value);
			}

			foreach (var stage in stages)
			{
				if (mappedTargetStates.TryGetValue(stage.id, out var statusItem))
				{
					stage.statusItem = statusItem;

					switch (statusItem.Id)
					{
						case "LookingUgly":
							stage.decor = uglyDecor;
							stage.cheerOnComplete = false;
							break;
						case "LookingOkay":
							stage.decor = okayDecor;
							stage.cheerOnComplete = false;
							break;
						case "LookingGreat":
							stage.decor = greatDecor;
							stage.cheerOnComplete = true;
							break;
						default:
							Log.Warning($"Invalid quality tier");
							break;
					}

					Log.Debug($"rearranged sculpture {stage.id} to {stage.statusItem.Id}");
				}
			}
		}

		// StatusItems are not added to the Db, so a simple Get() won't work
		private static ArtableStatusItem GetStatusItem(string value)
		{
			var statusItems = Db.Get().ArtableStatuses;

			switch (value)
			{
				case "Bad":
				case "LookingUgly":
					return statusItems.LookingUgly;
				case "Okay":
				case "LookingOkay":
					return statusItems.LookingOkay;
				case "Great":
				case "LookingGreat":
				default:
					return statusItems.LookingGreat;
			}
		}

		public static void RestoreStage(Artable instance, ref string currentStage)
		{
			if (instance.TryGetComponent(out ArtOverride artOverride) && !artOverride.overrideStage.IsNullOrWhiteSpace())
			{
				currentStage = artOverride.overrideStage;
			}
		}

		public static void UpdateOverride(Artable instance, string stage_id)
		{
			if (instance.TryGetComponent(out ArtOverride artOverride))
			{
				artOverride.UpdateOverride(stage_id);
			}
		}
	}
}
