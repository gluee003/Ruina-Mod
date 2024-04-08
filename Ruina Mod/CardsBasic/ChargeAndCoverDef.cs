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
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;

namespace Ruina_Mod.CardsBasic
{
    public sealed class ChargeAndCoverDef : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(ChargeAndCover);
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
              FindInBattle: true,
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
              Damage: 8,
              UpgradedDamage: 10,
              Block: 8,
              UpgradedBlock: 10,
              Shield: null,
              UpgradedShield: null,
              Value1: 1,
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

              Keywords: Keyword.Block,
              UpgradedKeywords: Keyword.Block,
              EmptyDescription: false,
              RelativeKeyword: Keyword.None,
              UpgradedRelativeKeyword: Keyword.None,

              RelativeEffects: new List<string>() { },
              UpgradedRelativeEffects: new List<string>() { "Weak" },
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

    [EntityLogic(typeof(ChargeAndCoverDef))]
    public sealed class ChargeAndCover : RuinaCard
    {
        public override AttackType attackType
        {
            get { return AttackType.Pierce; }
        }
        public override PageRange pageRange
        {
            get { return PageRange.Melee; }
        }
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            string text2;
            if (this.IsUpgraded)
            {
                string text;
                switch (consumingMana.MaxColor)
                {
                    case ManaColor.White:
                        text = "ShootW1";
                        goto IL_93;
                    case ManaColor.Blue:
                        text = "ShootU1";
                        goto IL_93;
                    case ManaColor.Black:
                        text = "ShootB1";
                        goto IL_93;
                    case ManaColor.Red:
                        text = "ShootR1";
                        goto IL_93;
                    case ManaColor.Green:
                        text = "ShootG1";
                        goto IL_93;
                    case ManaColor.Philosophy:
                        text = "ShootP1";
                        goto IL_93;
                }
                text = "ShootC1";
            IL_93:
                text2 = text;
            }
            else
            {
                string text;
                switch (consumingMana.MaxColor)
                {
                    case ManaColor.White:
                        text = "ShootW";
                        goto IL_101;
                    case ManaColor.Blue:
                        text = "ShootU";
                        goto IL_101;
                    case ManaColor.Black:
                        text = "ShootB";
                        goto IL_101;
                    case ManaColor.Red:
                        text = "ShootR";
                        goto IL_101;
                    case ManaColor.Green:
                        text = "ShootG";
                        goto IL_101;
                    case ManaColor.Philosophy:
                        text = "ShootP";
                        goto IL_101;
                }
                text = "ShootC";
            IL_101:
                text2 = text;
            }
            yield return base.AttackAction(selector, text2);
            yield return base.DefenseAction(true);

            if (this.IsUpgraded)
            {
                EnemyUnit selectedEnemy = selector.SelectedEnemy;
                if (selectedEnemy.IsAlive)
                {
                    yield return base.DebuffAction<Weak>(selectedEnemy, 0, base.Value1, 0, 0, true, 0.2f);
                }
                yield break;
            }
        }
    }
}

