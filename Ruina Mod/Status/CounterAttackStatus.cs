﻿using LBoL.Base;
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
using LBoL.Base.Extensions;
using System.Linq;
using static Ruina_Mod.RuinaCard;
using LBoL.EntityLib.StatusEffects.Basic;
using static Ruina_Mod.RolandPlayerDef;

namespace Ruina_Mod.Status
{
    public sealed class CounterAttackEffect : StatusEffectTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(CounterAttackStatus);
        }

        public override LocalizationOption LoadLocalization()
        {
            var loc = new GlobalLocalization(embeddedSource);
            loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "StatusEffectsEn.yaml");
            return loc;
        }

        public override Sprite LoadSprite()
        {
            return ResourceLoader.LoadSprite("CounterAttackSE.png", BepinexPlugin.embeddedSource);
        }

        public override StatusEffectConfig MakeConfig()
        {
            {
                var statusEffectConfig = new StatusEffectConfig(
                                Id: "",
                                Index: 0,
                                Order: 99,
                                Type: StatusEffectType.Positive,
                                IsVerbose: false,
                                IsStackable: false,
                                StackActionTriggerLevel: null,
                                HasLevel: true,
                                LevelStackType: StackType.Add,
                                HasDuration: false,
                                DurationStackType: StackType.Add,
                                DurationDecreaseTiming: DurationDecreaseTiming.Custom,
                                HasCount: false,
                                CountStackType: StackType.Keep,
                                LimitStackType: StackType.Keep,
                                ShowPlusByLimit: false,
                                Keywords: Keyword.None,
                                RelativeEffects: new List<string>() { },
                                VFX: "Default",
                                VFXloop: "Default",
                                SFX: "Default"
                    );
                return statusEffectConfig;
            }
        }
    }
    [EntityLogic(typeof(CounterAttackEffect))]
    public sealed class CounterAttackStatus : CounterStatus
    {
        protected override void OnAdded(Unit unit)
        {
            base.OnAdded(unit);
        }
        public override IEnumerable<BattleAction> TakeEffect(DamageEventArgs args)
        {
            activated = true;
            if (base.Level > unmodified_dmg)
            {
                yield return new DamageAction(base.Owner, args.Source, DamageInfo.Reaction((float)base.Level, false), "YoumuKan");
            }
            else if (base.Level == unmodified_dmg)
            {
                yield return new DamageAction(base.Owner, args.Source, DamageInfo.Reaction((float)base.Level, false), "YoumuKan");
                yield return new RemoveStatusEffectAction(this, true, 0.1f);
            }
            else if (base.Level < unmodified_dmg)
            {
                yield return new RemoveStatusEffectAction(this, true, 0.1f);
            }
            yield break;
        }
    }
}