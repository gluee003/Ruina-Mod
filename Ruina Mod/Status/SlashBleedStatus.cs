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
using static Ruina_Mod.RuinaCard;

namespace Ruina_Mod.Status
{
    public sealed class SlashBleedEffect : StatusEffectTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(SlashBleedStatus);
        }

        public override LocalizationOption LoadLocalization()
        {
            var loc = new GlobalLocalization(embeddedSource);
            loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "StatusEffectsEn.yaml");
            return loc;
        }

        public override Sprite LoadSprite()
        {
            return ResourceLoader.LoadSprite("SlashBleedSE.png", BepinexPlugin.embeddedSource);
        }

        public override StatusEffectConfig MakeConfig()
        {
            {
                var statusEffectConfig = new StatusEffectConfig(
                                Id: "",
                                Index: 0,
                                Order: 10,
                                Type: StatusEffectType.Positive,
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
                                RelativeEffects: new List<string>() { "BleedStatus" },
                                VFX: "Default",
                                VFXloop: "Default",
                                SFX: "Default"
                    );
                return statusEffectConfig;
            }
        }
    }
    [EntityLogic(typeof(SlashBleedEffect))]
    public sealed class SlashBleedStatus : StatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            base.ReactOwnerEvent<StatisticalDamageEventArgs>(base.Owner.StatisticalTotalDamageDealt, new EventSequencedReactor<StatisticalDamageEventArgs>(this.OnStatisticalDamageDealt));
        }

        private IEnumerable<BattleAction> OnStatisticalDamageDealt(StatisticalDamageEventArgs args)
        {
            if (base.Battle.BattleShouldEnd)
            {
                yield break;
            }
            foreach (KeyValuePair<Unit, IReadOnlyList<DamageEventArgs>> keyValuePair in args.ArgsTable)
            {
                Unit unit;
                IReadOnlyList<DamageEventArgs> readOnlyList;
                keyValuePair.Deconstruct(out unit, out readOnlyList);
                Unit unit2 = unit;
                IReadOnlyList<DamageEventArgs> readOnlyList2 = readOnlyList;

                foreach (DamageEventArgs damageAgs in readOnlyList2)
                {
                    if (unit2.IsAlive)
                    {
                        if (args.ActionSource is RuinaCard card && Owner.IsInTurn && card.CardType == CardType.Attack && card.attackType == AttackType.Slash)
                        {
                            DamageInfo damageInfo = damageAgs.DamageInfo;
                            if (damageInfo.Amount > 0f)
                            {
                                base.NotifyActivating();
                                yield return new ApplyStatusEffectAction<BleedStatus>(unit2, new int?(base.Level), null, null, null, 0f, true);
                            }
                        }
                    }
                }
            }
            yield break;
        }
    }
}
