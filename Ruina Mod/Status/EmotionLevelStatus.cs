using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.StatusEffects;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Ruina_Mod.BepinexPlugin;
using LBoL.Core.Units;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using System.Runtime.CompilerServices;

namespace Ruina_Mod.Status
{
    public sealed class EmotionLevelEffect : StatusEffectTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(EmotionLevelStatus);
        }

        public override LocalizationOption LoadLocalization()
        {
            var loc = new GlobalLocalization(embeddedSource);
            loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "StatusEffectsEn.yaml");
            return loc;
        }

        public override Sprite LoadSprite()
        {
            return ResourceLoader.LoadSprite("EmotionLevelSE.png", BepinexPlugin.embeddedSource);
        }
        public override ExtraIcons LoadExtraIcons()
        {
            ExtraIcons extraIcons = new ExtraIcons();

            Func<string, Sprite> wrap = (s) => ResourceLoader.LoadSprite(("EmotionLevelSE" + s + ".png"), embeddedSource);

            for (int i = 1; i <= 5; i++)
            {
                extraIcons.icons.Add(i.ToString(), wrap(i.ToString()));
            }

            return extraIcons;
        }

        public override StatusEffectConfig MakeConfig()
        {
            {
                var statusEffectConfig = new StatusEffectConfig(
                                Id: "",
                                Index: 0,
                                Order: 0,
                                Type: StatusEffectType.Special,
                                IsVerbose: false,
                                IsStackable: true,
                                StackActionTriggerLevel: null,
                                HasLevel: true,
                                LevelStackType: StackType.Add,
                                HasDuration: false,
                                DurationStackType: StackType.Add,
                                DurationDecreaseTiming: DurationDecreaseTiming.Custom,
                                HasCount: true,
                                CountStackType: StackType.Overwrite,
                                LimitStackType: StackType.Keep,
                                ShowPlusByLimit: false,
                                Keywords: Keyword.None,
                                RelativeEffects: new List<string>() { },
                                VFX: "BuffWhite",
                                VFXloop: "Default",
                                SFX: "Default"
                    );
                return statusEffectConfig;
            }
        }
    }
    [EntityLogic(typeof(EmotionLevelEffect))]
    public sealed class EmotionLevelStatus : StatusEffect
    {
        public int MaxEmotionLevel { get => 5; }
        public int EmotionLevel { get; set; }
        private int Threshold { get => thresholds[this.EmotionLevel]; }
        private readonly int[] thresholds = {3, 3, 5, 7, 9, 999999};
        public override string OverrideIconName
        {
            get
            {
                if (Battle == null)
                    return Id;

                if (this.EmotionLevel == 0)
                {
                    return Id;
                }
                else
                {
                    return Id + this.EmotionLevel.ToString();
                }
            }

        }
        public override bool Stack(StatusEffect other)
        {
            this.Level = Math.Min(this.Level + other.Level, this.Threshold);
            return true;
        }
        protected override void OnAdded(Unit unit)
        {
            this.EmotionLevel = 0;
            SetInitLevel(0);
            SetInitCount(this.Threshold);
            base.ReactOwnerEvent<UnitEventArgs>(unit.TurnStarting, new EventSequencedReactor<UnitEventArgs>(this.OnTurnStarting));
        }
        private IEnumerable<BattleAction> OnTurnStarting(UnitEventArgs args)
        {
            if (this.EmotionLevel < 5 && this.Level == this.Threshold)
            {
                this.EmotionLevel++;
                base.NotifyActivating();
                this.Level = 0;
                this.Count = this.Threshold;
            }
            yield break;
        }
    }
}