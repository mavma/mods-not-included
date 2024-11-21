﻿using Twitchery.Content.Scripts;
using UnityEngine;

namespace Twitchery.Content.Defs
{
	public class MidasEntityContainterConfig : IEntityConfig
	{
		public const string ID = "AkisTwitchEvents_MidasEntityContainer";

		public GameObject CreatePrefab()
		{
			var prefab = EntityTemplates.CreatePlacedEntity(
				ID,
				STRINGS.MISC.MIDAS_STATE,
				"",
				100f,
				Assets.GetAnim("barbeque_kanim"),
				"object",
				Grid.SceneLayer.Creatures,
				1,
				1,
				TUNING.DECOR.BONUS.TIER8);

			prefab.AddComponent<MinionStorage>();
			prefab.AddComponent<MidasEntityContainer>();
			var fx = prefab.AddComponent<HighlightFx>();
			fx.goalTintColor = Color.yellow;
			fx.duration = 3f;
			fx.soundFx = ModAssets.Sounds.GOLD;
			fx.soundTimestamp = (3f * 0.66f) - 1.24f;

			prefab.AddComponent<Storage>();
			SymbolOverrideControllerUtil.AddToPrefab(prefab);

			prefab.AddTag(ONITwitchLib.ExtraTags.OniTwitchSurpriseBoxForceDisabled);

			return prefab;
		}

		public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

		public void OnPrefabInit(GameObject inst) { }

		public void OnSpawn(GameObject inst) { }
	}
}
