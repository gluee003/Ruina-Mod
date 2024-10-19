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
    public sealed class DisarmEffect : StatusEffectTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(DisarmStatus);
        }

        public override LocalizationOption LoadLocalization()
        {
            var loc = new GlobalLocalization(embeddedSource);
            loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "StatusEffectsEn.yaml");
            return loc;
        }

        public override Sprite LoadSprite()
        {
            return ResourceLoader.LoadSprite("DisarmSE.png", BepinexPlugin.embeddedSource);
        }

        public override StatusEffectConfig MakeConfig()
        {
            {
                var statusEffectConfig = new StatusEffectConfig(
                                Id: "",
                                Index: 0,
                                Order: 999,
                                Type: StatusEffectType.Special,
                                IsVerbose: false,
                                IsStackable: true,
                                StackActionTriggerLevel: null,
                                HasLevel: false,
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
                                VFX: "DebuffRed",
                                VFXloop: "Default",
                                SFX: "FirepowerNegative"
                    );
                return statusEffectConfig;
            }
        }
    }
    [EntityLogic(typeof(DisarmEffect))]
    public sealed class DisarmStatus : StatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            foreach (Unit otherUnit in this.Battle.AllAliveUnits)
            {
                base.HandleOwnerEvent<DamageEventArgs>(otherUnit.DamageReceiving, new GameEventHandler<DamageEventArgs>(this.OnDamageReceiving));
            }
            base.ReactOwnerEvent<UnitEventArgs>(base.Owner.TurnEnded, new EventSequencedReactor<UnitEventArgs>(this.OnOwnerTurnEnded));
        }
        private void OnDamageReceiving(DamageEventArgs args)
        {
            DamageInfo damageInfo = args.DamageInfo;
            if (args.Source == this.Owner && damageInfo.DamageType == DamageType.Attack)
            {
                args.DamageInfo = damageInfo.MultiplyBy(0);
                args.AddModifier(this);
            }
        }
        private IEnumerable<BattleAction> OnOwnerTurnEnded(UnitEventArgs args)
        {
            yield return new RemoveStatusEffectAction(this, true, 0.1f);
        }
    }
}

