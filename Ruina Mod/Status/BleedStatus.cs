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

namespace Ruina_Mod.Status
{
    public sealed class BleedEffect : StatusEffectTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(BleedStatus);
        }

        public override LocalizationOption LoadLocalization()
        {
            var loc = new GlobalLocalization(embeddedSource);
            loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "StatusEffectsEn.yaml");
            return loc;
        }

        public override Sprite LoadSprite()
        {
            return ResourceLoader.LoadSprite("BleedSE.png", BepinexPlugin.embeddedSource);
        }

        public override StatusEffectConfig MakeConfig()
        {
            {
                var statusEffectConfig = new StatusEffectConfig(
                                Id: "",
                                Index: 0,
                                Order: 10,
                                Type: StatusEffectType.Negative,
                                IsVerbose: false,
                                IsStackable: true,
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
    [EntityLogic(typeof(BleedEffect))]
    public sealed class BleedStatus : StatusEffect
    {
        //int num_attacks = 0;
        int Threshold = 3;
        protected override void OnAdded(Unit unit)
        {
            //base.ReactOwnerEvent<DamageEventArgs>(Battle.Player.DamageReceived, new EventSequencedReactor<DamageEventArgs>(this.OnDamageReceived));
            //base.ReactOwnerEvent<StatisticalDamageEventArgs>(unit.StatisticalTotalDamageDealt, new EventSequencedReactor<StatisticalDamageEventArgs>(this.OnStatisticalDamageDealt));
            base.ReactOwnerEvent<DamageEventArgs>(unit.DamageDealt, new EventSequencedReactor<DamageEventArgs>(this.OnDamageDealt));
            base.ReactOwnerEvent<UnitEventArgs>(unit.TurnEnding, new EventSequencedReactor<UnitEventArgs>(this.OnOwnerTurnEnding));
        }
        //private IEnumerable<BattleAction> OnDamageReceived(DamageEventArgs args)
        //{
        //    if (args.Source.IsInTurn && args.DamageInfo.DamageType == DamageType.Attack && args.Source.GetStatusEffect<BleedStatus>() == this)
        //    {
        //        num_attacks++;
        //    }
        //    yield break;
        //}
        //private IEnumerable<BattleAction> OnStatisticalDamageDealt(StatisticalDamageEventArgs args)
        //{
        //    for (int i = num_attacks; i > 0; i--)
        //    {
        //        if (base.Owner == null || base.Battle.BattleShouldEnd)
        //        {
        //            yield break;
        //        }
        //        base.NotifyActivating();
        //        yield return DamageAction.LoseLife(base.Owner, base.Level);
        //        double num = Math.Ceiling(base.Level / 1.5);
        //        base.Level = (int)num;
        //    }
        //    num_attacks = 0;
        //    yield break;
        //}
        private IEnumerable<BattleAction> OnDamageDealt(DamageEventArgs args)
        {
            if (base.Owner == null || base.Battle.BattleShouldEnd)
            {
                yield break;
            }
            if (args.Source.IsInTurn && args.DamageInfo.DamageType == DamageType.Attack)
            {   
                base.NotifyActivating();
                yield return DamageAction.LoseLife(base.Owner, base.Level);
                yield return new PerformAction(new PerformAction.GunArgs { Source = args.Source, Target = args.Source, GunId = "RemiZhua", OccupationTime = 0.1f });
                double num = Math.Ceiling(base.Level / 1.5);
                base.Level = (int)num;
            }
            yield break;
        }
        private IEnumerable<BattleAction> OnOwnerTurnEnding(UnitEventArgs args)
        {
            if (base.Level < Threshold)
            {
                yield return new RemoveStatusEffectAction(this, true, 0.3f);
            }
            yield break;
        }
    }
}