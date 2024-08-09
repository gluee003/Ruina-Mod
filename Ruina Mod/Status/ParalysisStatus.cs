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
using LBoL.Base.Extensions;
using System.Linq;
using static Ruina_Mod.RuinaCard;
using LBoL.EntityLib.StatusEffects.Basic;

namespace Ruina_Mod.Status
{
    public sealed class ParalysisEffect : StatusEffectTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(ParalysisStatus);
        }

        public override LocalizationOption LoadLocalization()
        {
            var loc = new GlobalLocalization(embeddedSource);
            loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "StatusEffectsEn.yaml");
            return loc;
        }

        public override Sprite LoadSprite()
        {
            return ResourceLoader.LoadSprite("ParalysisSE.png", BepinexPlugin.embeddedSource);
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
    [EntityLogic(typeof(ParalysisEffect))]
    public sealed class ParalysisStatus : StatusEffect
    {
        public int Value = 3;
        public int Value2 = 1;
        protected override void OnAdded(Unit unit)
        {
            base.HandleOwnerEvent<DamageDealingEventArgs>(unit.DamageDealing, new GameEventHandler<DamageDealingEventArgs>(this.OnDamageDealing));
            base.ReactOwnerEvent<DamageEventArgs>(unit.DamageDealt, new EventSequencedReactor<DamageEventArgs>(this.OnDamageDealt));
            base.HandleOwnerEvent<BlockShieldEventArgs>(unit.BlockShieldGaining, new GameEventHandler<BlockShieldEventArgs>(this.OnBlockGaining));
            base.HandleOwnerEvent<BlockShieldEventArgs>(unit.BlockShieldCasting, new GameEventHandler<BlockShieldEventArgs>(this.OnBlockCasting));
            base.ReactOwnerEvent<BlockShieldEventArgs>(unit.BlockShieldGained, new EventSequencedReactor<BlockShieldEventArgs>(this.OnBlockShieldGained));
            base.ReactOwnerEvent<StatusEffectApplyEventArgs>(unit.StatusEffectAdding, new EventSequencedReactor<StatusEffectApplyEventArgs>(this.OnStatusEffectAdding));
            base.ReactOwnerEvent<StatusEffectApplyEventArgs >(unit.StatusEffectAdded, new EventSequencedReactor<StatusEffectApplyEventArgs>(this.OnStatusEffectAdded));
        }
        private void OnDamageDealing(DamageDealingEventArgs args)
        {
            if (args.DamageInfo.DamageType == DamageType.Attack && base.Level > 0)
            {
                args.DamageInfo = args.DamageInfo.ReduceBy(this.Value);
                args.AddModifier(this);
            }
        }
        private void OnBlockGaining(BlockShieldEventArgs args)
        {
            if (args.Type == BlockShieldType.Direct)
            {
                return;
            }
            ActionCause cause = args.Cause;
            if (cause == ActionCause.Card || cause == ActionCause.OnlyCalculate || cause == ActionCause.Us)
            {
                if (args.Block != 0f)
                {
                    args.Block = Math.Max(args.Block - (float)this.Value, 0f);
                }
                if (args.Shield != 0f)
                {
                    args.Shield = Math.Max(args.Shield - (float)this.Value, 0f);
                }
                args.AddModifier(this);
            }
        }
        private void OnBlockCasting(BlockShieldEventArgs args)
        {
            if (args.Type == BlockShieldType.Direct)
            {
                return;
            }
            if (args.Cause == ActionCause.EnemyAction)
            {
                if (args.Block != 0f)
                {
                    args.Block = Math.Max(args.Block - (float)this.Value, 0f);
                }
                if (args.Shield != 0f)
                {
                    args.Shield = Math.Max(args.Shield - (float)this.Value, 0f);
                }
                args.AddModifier(this);
            }
        }
        private IEnumerable<BattleAction> OnDamageDealt(DamageEventArgs args)
        {
            if (args.DamageInfo.DamageType == DamageType.Attack && base.Level > 0)
            {
                base.NotifyActivating();
                base.Level--;
            }
            if (base.Level == 0)
            {
                yield return new RemoveStatusEffectAction(this, true, 0.1f);
            }
            yield break;
        }
        private IEnumerable<BattleAction> OnBlockShieldGained(BlockShieldEventArgs args)
        {
            ActionCause cause = args.Cause;
            Debug.Log(cause);
            if (args.Type == BlockShieldType.Direct)
            {
                yield break;
            }
            //ActionCause cause = args.Cause;
            if ((cause == ActionCause.Card || cause == ActionCause.OnlyCalculate || cause == ActionCause.Us || cause == ActionCause.EnemyAction) && base.Level > 0)
            {
                base.NotifyActivating();
                base.Level--;
            }
            if (base.Level == 0)
            {
                yield return new RemoveStatusEffectAction(this, true, 0.1f);
            }
            yield break;
        }
        private IEnumerable<BattleAction> OnStatusEffectAdding(StatusEffectApplyEventArgs args)
        {
            StatusEffect effect = args.Effect;
            if (effect is Graze)
            {
                base.NotifyActivating();
                if (args.Effect.Level <= 1)
                {
                    args.CancelBy(this);
                    base.Level--;
                    if (base.Level == 0)
                    {
                        yield return new RemoveStatusEffectAction(this, true, 0.1f);
                    }
                }
                else if (args.Effect.Level > 1)
                {
                    args.Effect.Level -= 1;
                }
            }
            yield break;
        }
        private IEnumerable<BattleAction> OnStatusEffectAdded(StatusEffectApplyEventArgs args)
        {
            StatusEffect effect = args.Effect;
            if (effect is Graze)
            {
                base.Level--;
            }
            if (base.Level == 0)
            {
                yield return new RemoveStatusEffectAction(this, true, 0.1f);
            }
            yield break;
        }
        public override string UnitEffectName
        {
            get
            {
                return "ElectricLoop";
            }
        }
    }
}
