using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.Battle;
using LBoL.Core;
using LBoL.Core.Cards;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using static Ruina_Mod.BepinexPlugin;
using LBoL.Core.Battle.BattleActions;

namespace Ruina_Mod.CardsBasic
{
    public sealed class LightDefenseDef : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(LightDefense);
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
              Type: CardType.Defense,
              TargetType: TargetType.Nobody,
              Colors: new List<ManaColor>() { ManaColor.Colorless },
              IsXCost: false,
              Cost: new ManaGroup() { Any = 2 },
              UpgradedCost: null,
              MoneyCost: null,
              Damage: null,
              UpgradedDamage: null,
              Block: 10,
              UpgradedBlock: 14,
              Shield: null,
              UpgradedShield: null,
              Value1: null,
              UpgradedValue1: null,
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

              Keywords: Keyword.Basic | Keyword.Block,
              UpgradedKeywords: Keyword.Basic | Keyword.Block,
              EmptyDescription: false,
              RelativeKeyword: Keyword.None,
              UpgradedRelativeKeyword: Keyword.None,

              RelativeEffects: new List<string>() { },
              UpgradedRelativeEffects: new List<string>() { },
              RelativeCards: new List<string>() { },
              UpgradedRelativeCards: new List<string>() { },

              Owner: null,
              ImageId: "",
              UpgradeImageId: "",

              Unfinished: false,
              Illustrator: null,
              SubIllustrator: new List<string>() { }
           );


            return cardConfig;
        }
    }

    [EntityLogic(typeof(LightDefenseDef))]
    public sealed class LightDefense : Card
    {
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return DefenseAction(true);
            yield break;
        }
    }
}
