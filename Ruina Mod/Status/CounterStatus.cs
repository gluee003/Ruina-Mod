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
    public class CounterEffect : StatusEffectTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(CounterStatus);
        }

        public override LocalizationOption LoadLocalization()
        {
            var loc = new GlobalLocalization(embeddedSource);
            loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "StatusEffectsEn.yaml");
            return loc;
        }

        public override Sprite LoadSprite()
        {
            return ResourceLoader.LoadSprite("CounterSE.png", BepinexPlugin.embeddedSource);
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
    [EntityLogic(typeof(CounterEffect))]
    public abstract class CounterStatus : StatusEffect
    {
        public bool activated = false;
        public float unmodified_dmg;
        public int num_evaded = 0;
        protected override void OnAdded(Unit unit)
        {
            base.HandleOwnerEvent<DamageEventArgs>(unit.DamageTaking, new GameEventHandler<DamageEventArgs>(this.OnDamageTaking));
            base.ReactOwnerEvent<DamageEventArgs>(unit.DamageReceived, new EventSequencedReactor<DamageEventArgs>(this.OnDamageReceived));
            base.ReactOwnerEvent<StatisticalDamageEventArgs>(unit.StatisticalTotalDamageReceived, new EventSequencedReactor<StatisticalDamageEventArgs>(this.OnStatisticalDamageReceived));
            base.ReactOwnerEvent<UnitEventArgs>(unit.TurnStarting, new EventSequencedReactor<UnitEventArgs>(this.OnTurnStarting));
        }
        private void OnDamageTaking(DamageEventArgs args)
        {
            List<StatusEffect> status_effects = new List<StatusEffect>();
            foreach (StatusEffect statusEffect in Owner.StatusEffects.Where((effect) => effect is CounterStatus))
            {
                status_effects.Add(statusEffect);
            }
            unmodified_dmg = args.DamageInfo.Amount;
            if (System.Object.ReferenceEquals(this, status_effects[^1]) && args.Source != base.Owner && args.Source.IsAlive && args.DamageInfo.DamageType == DamageType.Attack && unmodified_dmg > 0)
            {
                CounterStatus activating_status = Owner.GetStatusEffectExtend<CounterStatus>();
                if (activating_status is CounterBlockStatus)
                // if activating status is block, reduce damage taken by level 
                {
                    float dmg = Math.Max(0f, unmodified_dmg - activating_status.Level);
                    if (dmg == 0f)
                    {
                        args.DamageInfo = new DamageInfo(dmg, args.DamageInfo.DamageType, isGrazed: false, isAccuracy: false, dontBreakPerfect: args.DamageInfo.DontBreakPerfect);
                    }
                    else
                    {
                        args.DamageInfo = new DamageInfo(dmg, args.DamageInfo.DamageType, isGrazed: args.DamageInfo.IsGrazed, isAccuracy: args.DamageInfo.IsAccuracy, dontBreakPerfect: args.DamageInfo.DontBreakPerfect);
                    }
                    args.DamageInfo = args.DamageInfo.BlockBy(args.Target.Block);
                    args.DamageInfo = args.DamageInfo.ShieldBy(args.Target.Shield);
                    args.AddModifier(this);
                    activating_status.NotifyActivating();
                }
                else
                // else if activating status is attack/evade, nullify damage only if level >= damage
                {
                    if (activating_status.Level >= unmodified_dmg)
                    {
                        args.DamageInfo = new DamageInfo(0f, args.DamageInfo.DamageType, isGrazed: false, isAccuracy: false, dontBreakPerfect: args.DamageInfo.DontBreakPerfect);
                        args.AddModifier(this);
                        activating_status.NotifyActivating();
                        if (activating_status is CounterEvadeStatus && activating_status.Level > unmodified_dmg)
                        {
                            if (activating_status.num_evaded < 4)
                            {
                                activating_status.num_evaded++;
                            }
                        }
                    }
                }
            }
        }
        public abstract IEnumerable<BattleAction> TakeEffect(DamageEventArgs args);

        private IEnumerable<BattleAction> OnDamageReceived(DamageEventArgs args)
        {
            List<StatusEffect> status_effects = new List<StatusEffect>();
            foreach (StatusEffect statusEffect in Owner.StatusEffects.Where((effect) => effect is CounterStatus))
            {
                status_effects.Add(statusEffect);
            }
            if (System.Object.ReferenceEquals(this, status_effects[^1]) && args.Source != base.Owner && args.Source.IsAlive && args.DamageInfo.DamageType == DamageType.Attack && unmodified_dmg > 0)
            {
                CounterStatus activating_status = Owner.GetStatusEffectExtend<CounterStatus>();
                IEnumerable<BattleAction> actions = activating_status.TakeEffect(args);
                foreach (BattleAction action in actions)
                {
                    yield return action;
                }
            }
            yield break;
        }
        private IEnumerable<BattleAction> OnStatisticalDamageReceived(StatisticalDamageEventArgs args)
        {
            if (activated)
            {
                CounterStatus activating_status = Owner.GetStatusEffectExtend<CounterStatus>();
                if (activating_status is CounterEvadeStatus activating_evade_status && activating_evade_status.num_evaded > 0)
                {
                    yield return new ApplyStatusEffectAction<Graze>(base.Owner, activating_evade_status.num_evaded, null, null, null, 0.1f);
                }
                yield return new RemoveStatusEffectAction(activating_status, true, 0.1f);
            }
            yield break;
        }
        private IEnumerable<BattleAction> OnTurnStarting(UnitEventArgs args)
        {
            base.NotifyActivating();
            yield return new RemoveStatusEffectAction(this, true, 0.1f);
            yield break;
        }
        public string Active
        {
            get
            {
                if (System.Object.ReferenceEquals(this, Owner.GetStatusEffectExtend<CounterStatus>()))
                {
                    return "Active";
                }
                else
                {
                    return "Inactive";
                }
            }
        }
    }
}