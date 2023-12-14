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

namespace Ruina_Mod.CardsC
{
    public sealed class FlamingBatDef : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(FlamingBat);
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
              Damage: 5,
              UpgradedDamage: 5,
              Block: 5,
              UpgradedBlock: 5,
              Shield: null,
              UpgradedShield: null,
              Value1: 1,
              UpgradedValue1: null,
              Value2: 5,
              UpgradedValue2: 8,
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

              Keywords: Keyword.Block,
              UpgradedKeywords: Keyword.Block,
              EmptyDescription: false,
              RelativeKeyword: Keyword.None,
              UpgradedRelativeKeyword: Keyword.None,

              RelativeEffects: new List<string>() { "Graze" , "BurnStatus" },
              UpgradedRelativeEffects: new List<string>() { "Graze" , "BurnStatus"},
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
    [EntityLogic(typeof(FlamingBatDef))]
    public sealed class FlamingBat : Card
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return base.DefenseAction(true);
            yield return base.BuffAction<Graze>(base.Value1, 0, 0, 0, 0.2f);
            yield return base.AttackAction(selector.SelectedEnemy);
            yield return new ApplyStatusEffectAction<BurnStatus>(selector.SelectedEnemy, new int?(base.Value2), null, null, null, 0f, true);
            yield break;
        }
    }
}
