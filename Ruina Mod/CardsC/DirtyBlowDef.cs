using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using Mono.Cecil;
using Ruina_Mod.CardsBasic;
using System;
using System.Collections.Generic;
using System.Text;
using static Ruina_Mod.BepinexPlugin;
using LBoL.Core.Battle.BattleActions;
using Ruina_Mod.Status;
using LBoL.EntityLib.StatusEffects.Others;
using LBoL.Core.StatusEffects;
using System.Linq;
using LBoL.Core.Units;
using static UnityEngine.UI.CanvasScaler;
using UnityEngine;

namespace Ruina_Mod.CardsC
{
    public sealed class DirtyBlowDef : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(DirtyBlow);
        }

        public override CardImages LoadCardImages()
        {
            var imgs = new CardImages(embeddedSource);
            imgs.AutoLoad(this, extension: ".png");
            return imgs;
        }

        public override LocalizationOption LoadLocalization()
        {
            var loc = new GlobalLocalization(embeddedSource);
            loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "CardsEn.yaml");
            return loc;
        }

        public override CardConfig MakeConfig()
        {
            var cardConfig = new CardConfig(
              Index: BepinexPlugin.sequenceTable.Next(typeof(CardConfig)),
              Id: "",
              Order: 10,
              AutoPerform: true,
              Perform: new string[0][],
              GunName: "Simple1",
              GunNameBurst: "Simple2",
              DebugLevel: 0,
              Revealable: false,
              IsPooled: true,
              HideMesuem: false,
              IsUpgradable: true,
              Rarity: Rarity.Common,
              Type: CardType.Attack,
              TargetType: TargetType.SingleEnemy,
              Colors: new List<ManaColor>() { ManaColor.Colorless },
              IsXCost: false,
              Cost: new ManaGroup() { Any = 2 },
              UpgradedCost: null,
              MoneyCost: null,
              Damage: 10,
              UpgradedDamage: 13,
              Block: null,
              UpgradedBlock: null,
              Shield: null,
              UpgradedShield: null,
              Value1: 3,
              UpgradedValue1: 5,
              Value2: null,
              UpgradedValue2: null,
              Mana: null,
              UpgradedMana: null,
              Scry: null,
              UpgradedScry: null,

              ToolPlayableTimes: null,

              Loyalty: null,
              UpgradedLoyalty: null,
              PassiveCost: null,
              UpgradedPassiveCost: null,
              ActiveCost: null,
              UpgradedActiveCost: null,
              UltimateCost: null,
              UpgradedUltimateCost: null,

              Keywords: Keyword.None,
              UpgradedKeywords: Keyword.None,
              EmptyDescription: false,
              RelativeKeyword: Keyword.None,
              UpgradedRelativeKeyword: Keyword.None,

              RelativeEffects: new List<string>() { "BleedStatus" },
              UpgradedRelativeEffects: new List<string>() { "BleedStatus" },
              RelativeCards: new List<string>() { },
              UpgradedRelativeCards: new List<string>() { },

              Owner: "Roland",
              ImageId: "",
              UpgradeImageId: "",

              Unfinished: false,
              Illustrator: "Nai_ Ga",
              SubIllustrator: new List<string>() { }
           );


            return cardConfig;
        }
    }
    [EntityLogic(typeof(DirtyBlowDef))]
    public sealed class DirtyBlow : RuinaCard
    {
        public override AttackType attackType
        {
            get { return AttackType.Blunt; }
        }
        public override PageRange pageRange
        {
            get { return PageRange.Melee; }
        }
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return base.AttackAction(selector.SelectedEnemy);
            yield break;
        }

        protected override void OnEnterBattle(BattleController battle)
        {
            base.ReactBattleEvent<StatisticalDamageEventArgs>(base.Battle.Player.StatisticalTotalDamageDealt, new EventSequencedReactor<StatisticalDamageEventArgs>(this.OnStatisticalDamageDealt));
        }

        private IEnumerable<BattleAction> OnStatisticalDamageDealt(StatisticalDamageEventArgs args)
        {
            if (base.Battle.BattleShouldEnd || args.ActionSource != this)
            {
                yield break;
            }
            foreach (KeyValuePair<LBoL.Core.Units.Unit, IReadOnlyList<DamageEventArgs>> keyValuePair in args.ArgsTable)
            {
                LBoL.Core.Units.Unit unit;
                IReadOnlyList<DamageEventArgs> readOnlyList;
                keyValuePair.Deconstruct(out unit, out readOnlyList);
                LBoL.Core.Units.Unit unit2 = unit;
                IReadOnlyList<DamageEventArgs> readOnlyList2 = readOnlyList;

                foreach (DamageEventArgs damageAgs in readOnlyList2)
                {
                    if (unit2.IsAlive)
                    {
                        DamageInfo damageInfo = damageAgs.DamageInfo;
                        if (damageInfo.Amount > 0f)
                        {
                            yield return new ApplyStatusEffectAction<BleedStatus>(unit2, new int?(base.Value1), null, null, null, 0f, true);
                        }
                    }
                }
            }
            yield break;
        }
    }
}