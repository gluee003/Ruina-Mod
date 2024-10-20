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
        public int HitEmotion { get => 1; }
        public int EnemyDeathEmotion { get => 3; }
        public int MaxEmotionLevel { get => 5; }
        public int EmotionLevel { get; set; }
        public int BonusMana
        {
            get
            {
                if (this.Battle == null)
                {
                    return 0;
                }
                switch (this.EmotionLevel)
                {
                    case 2: 
                    case 3:
                        return 1;
                    case 4:
                    case 5:
                        return 2;
                    default:
                        return 0;
                }
            }
        }
        public int BonusDraw
        {
            get
            {
                if (this.Battle == null)
                {
                    return 0;
                }
                switch (this.EmotionLevel)
                {
                    case 3:
                    case 4:
                        return 1;
                    case 5:
                        return 2;
                    default:
                        return 0;
                }
            }
        }
        private int Threshold { get => thresholds[this.EmotionLevel]; }
        private readonly int[] thresholds = {3, 5, 7, 9, 11, 999999};
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
        public string DefaultSelfName { get => StringDecorator.Decorate($"|{this.LocalizeProperty("Name")}|"); }
        public override string Name
        {
            get
            {
                if (this.Battle == null)
                {
                    return base.Name;
                }
                else
                {
                    return base.Name + " " + this.EmotionLevel.ToString();
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
            base.ReactOwnerEvent<UnitEventArgs>(unit.TurnStarted, new EventSequencedReactor<UnitEventArgs>(this.OnTurnStarted));
            base.ReactOwnerEvent<DamageEventArgs>(unit.DamageDealt, new EventSequencedReactor<DamageEventArgs>(this.OnDamageDealt));
            base.ReactOwnerEvent<DieEventArgs>(this.Battle.EnemyDied, new EventSequencedReactor<DieEventArgs>(this.OnEnemyDied));
            base.ReactOwnerEvent<DamageEventArgs>(unit.DamageReceived, new EventSequencedReactor<DamageEventArgs>(this.OnDamageReceived));
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
        private IEnumerable<BattleAction> OnTurnStarted(UnitEventArgs args)
        {
            if (!base.Battle.BattleShouldEnd)
            {
                if (this.BonusMana > 0)
                {
                    base.NotifyActivating();
                    yield return new GainManaAction(ManaGroup.Colorlesses(this.BonusMana));
                }
                if (this.BonusDraw > 0)
                {
                    base.NotifyActivating();
                    yield return new DrawManyCardAction(this.BonusDraw);
                }
            }
        }
        private IEnumerable<BattleAction> OnDamageDealt(DamageEventArgs args)
        {
            if (args.DamageInfo.DamageType == DamageType.Attack && args.DamageInfo.Damage > 0f)
            {
                if (args.ActionSource is Card card && card.Config.TargetType == TargetType.AllEnemies)
                {
                    yield break;
                }
                yield return new ApplyStatusEffectAction<EmotionLevelStatus>(this.Owner, this.HitEmotion);
            }
        }
        private IEnumerable<BattleAction> OnEnemyDied(DieEventArgs args)
        {
            if (!this.Battle.BattleShouldEnd)
            {
                yield return new ApplyStatusEffectAction<EmotionLevelStatus>(this.Owner, this.EnemyDeathEmotion);
            }
        }
        private IEnumerable<BattleAction> OnDamageReceived(DamageEventArgs args)
        {
            if (args.DamageInfo.DamageType == DamageType.Attack && args.DamageInfo.Amount > 0f)
            {
                yield return new ApplyStatusEffectAction<EmotionLevelStatus>(this.Owner, this.HitEmotion);
            }
        }
    }
}