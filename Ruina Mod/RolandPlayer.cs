using Cysharp.Threading.Tasks;
using LBoL.ConfigData;
using LBoL.Core.Units;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.Utils;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Ruina_Mod.BepinexPlugin;

namespace Ruina_Mod
{
    public sealed class RolandPlayerDef : PlayerUnitTemplate
    {
        public static DirectorySource dir = new DirectorySource(PluginInfo.GUID, "Roland");

        public static string name = nameof(Roland);

        public override IdContainer GetId()
        {
            return nameof(Roland);
        }

        public override LocalizationOption LoadLocalization()
        {
            var loc = new GlobalLocalization(embeddedSource);
            loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "PlayerUnitEn.yaml");
            return loc;
        }

        public override PlayerImages LoadPlayerImages()
        {
            var sprites = new PlayerImages();

            var asyncLoading = ResourceLoader.LoadSpriteAsync("Roland.png", directorySource);
            var loading2 = ResourceLoader.LoadSprite("CardImprint.png", directorySource);
            var loading3 = ResourceLoader.LoadSprite("Avatar.png", directorySource);
            var loading4 = ResourceLoader.LoadSprite("CollectionIcon.png", directorySource);

            sprites.SetStartPanelStand(asyncLoading);
            sprites.SetWinStand(asyncLoading);
            sprites.SetDeckStand(asyncLoading);
            sprites.SetCardImprint(() => loading2);
            sprites.SetInRunAvatarPic(() => loading3);
            sprites.SetCollectionIcon(() => loading4);

            return sprites;
        }

        public override PlayerUnitConfig MakeConfig()
        {
            var config = new PlayerUnitConfig(
            Id: "",
            ShowOrder: 6,
            Order: 0,
            UnlockLevel: 0,
            ModleName: "",
            NarrativeColor: "#CCCCCC",
            IsSelectable: true,
            MaxHp: 80,
            InitialMana: new LBoL.Base.ManaGroup() { Colorless = 4 },
            InitialMoney: 10,
            InitialPower: 0,
            UltimateSkillA: "ReimuUltR",
            UltimateSkillB: "ReimuUltW",
            ExhibitA: "Durandal",
            ExhibitB: "PerceptionMask",
            DeckA: new List<string> { "Pound", "Pound", "Evade", "Evade", "LightAttack", "LightAttack", "LightDefense", "LightDefense", "LightDefense", "ChargeAndCover" },
            DeckB: new List<string> { "Pound", "Pound", "Evade", "Evade", "LightAttack", "LightAttack", "LightDefense", "LightDefense", "LightDefense", "FocusedStrikes" },
            DifficultyA: 2,
            DifficultyB: 2
            );
            return config;
        }

        [EntityLogic(typeof(RolandPlayerDef))]
        public sealed class Roland : PlayerUnit 
        { 
        }
    }

    public sealed class RolandModelDef : UnitModelTemplate
    {
        public override IdContainer GetId()
        {
            return new RolandPlayerDef().UniqueId;
        }

        public override LocalizationOption LoadLocalization()
        {
            var loc = new GlobalLocalization(embeddedSource);
            loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "PlayerModelEn.yaml");
            return loc;
        }

        public override ModelOption LoadModelOptions()
        {
            return new ModelOption(ResourceLoader.LoadSpriteAsync("Roland_Sprite.png", directorySource, 250));
        }

        public override UniTask<Sprite> LoadSpellSprite() => ResourceLoader.LoadSpriteAsync("Roland.png", RolandPlayerDef.dir);

        public override UnitModelConfig MakeConfig()
        {
            var config = UnitModelConfig.FromName("Reimu").Copy();
            config.Flip = false;
            config.Type = 0;
            config.Offset = new Vector2(0, 0.04f);
            return config;
        }
    }
}
