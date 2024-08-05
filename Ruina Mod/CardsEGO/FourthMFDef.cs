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
using LBoL.EntityLib.StatusEffects.Cirno;

namespace Ruina_Mod.CardsC
{
    public sealed class FourthMFDef : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(FourthMF);
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
              GunName: "秦心红",
              GunNameBurst: "秦心红",
              DebugLevel: 0,
              Revealable: false,
              IsPooled: true,
              FindInBattle: true,
              HideMesuem: false,
              IsUpgradable: true,
              Rarity: Rarity.Rare,
              Type: CardType.Attack,
              TargetType: TargetType.AllEnemies,
              Colors: new List<ManaColor>() { ManaColor.Red },
              IsXCost: false,
              Cost: new ManaGroup() { Any = 5 },
              UpgradedCost: null,
              MoneyCost: null,
              Damage: 32,
              UpgradedDamage: 40,
              Block: null,
              UpgradedBlock: null,
              Shield: null,
              UpgradedShield: null,
              Value1: 10,
              UpgradedValue1: 16,
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

              Keywords: Keyword.Accuracy | Keyword.Retain | Keyword.Exile,
              UpgradedKeywords: Keyword.Accuracy | Keyword.Retain | Keyword.Exile,
              EmptyDescription: false,
              RelativeKeyword: Keyword.None,
              UpgradedRelativeKeyword: Keyword.None,

              RelativeEffects: new List<string>() { "BurnStatus" },
              UpgradedRelativeEffects: new List<string>() { "BurnStatus" },
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
    [EntityLogic(typeof(FourthMFDef))]
    public sealed class FourthMF : RuinaCard
    {
        public override AttackType attackType
        {
            get { return AttackType.Slash; }
        }
        public override PageRange pageRange
        {
            get { return PageRange.Melee; }
        }
        public override PageType pageType
        {
            get { return PageType.EGO;  }
        }
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return base.AttackAction(selector);
            foreach (BattleAction battleAction in base.DebuffAction<BurnStatus>(base.Battle.AllAliveEnemies, base.Value1, 0, 0, 0, false, 0.1f))
            {
                yield return battleAction;
            }
            yield break;
        }
    }
}

