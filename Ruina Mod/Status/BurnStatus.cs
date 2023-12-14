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
    public sealed class BurnEffect : StatusEffectTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(BurnStatus);
        }

        public override LocalizationOption LoadLocalization()
        {
            var loc = new GlobalLocalization(embeddedSource);
            loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "StatusEffectsEn.yaml");
            return loc;
        }

        public override Sprite LoadSprite()
        {
            return ResourceLoader.LoadSprite("BurnSE.png", BepinexPlugin.embeddedSource);
        }

        public override StatusEffectConfig MakeConfig()
        {
            {
                var statusEffectConfig = new StatusEffectConfig(
                                Id: "",
                                Index: 0,
                                Order: 10,
                                Type: StatusEffectType.Special,
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
    [EntityLogic(typeof(BurnEffect))]
    public sealed class BurnStatus : StatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            if (unit is EnemyUnit)
            {
                base.ReactOwnerEvent<GameEventArgs>(base.Battle.AllEnemyTurnStarted, new EventSequencedReactor<GameEventArgs>(this.OnAllEnemyTurnStarted));
                base.ReactOwnerEvent<UnitEventArgs>(base.Owner.TurnStarted, new EventSequencedReactor<UnitEventArgs>(this.OnEnemyTurnStarted));
                return;
            }
            base.ReactOwnerEvent<UnitEventArgs>(base.Owner.TurnStarted, new EventSequencedReactor<UnitEventArgs>(this.OnPlayerTurnStarted));
        }

        private IEnumerable<BattleAction> OnAllEnemyTurnStarted(GameEventArgs args)
        {
            return this.TakeEffect();
        }

        private IEnumerable<BattleAction> OnEnemyTurnStarted(UnitEventArgs args)
        {
            Unit owner = base.Owner;
            if (owner == null || !owner.IsExtraTurn)
            {
                yield break;
            }
            foreach (BattleAction battleAction in this.TakeEffect())
            {
                yield return battleAction;
            }
            yield break;
        }

        private IEnumerable<BattleAction> OnPlayerTurnStarted(UnitEventArgs args)
        {
            return this.TakeEffect();
        }

        public IEnumerable<BattleAction> TakeEffect()
        {
            if (base.Owner == null || base.Battle.BattleShouldEnd)
            {
                yield break;
            }
            base.NotifyActivating();
            yield return DamageAction.Reaction(base.Owner, base.Level);
            double num = Math.Floor(base.Level / 1.5);
            base.Level = (int)num;
            if (base.Level == 0)
            {
                yield return new RemoveStatusEffectAction(this, true, 0.1f);
            }
            yield break;
        }
    }
}
