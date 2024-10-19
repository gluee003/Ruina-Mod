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
using LBoL.Core.Intentions;
using static UnityEngine.UI.CanvasScaler;
using UnityEngine;

namespace Ruina_Mod.CardsC
{
    public sealed class BoundaryOfDeathDef : CardTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(BoundaryOfDeath);
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
              Order: 9999,
              AutoPerform: true,
              Perform: new string[0][],
              GunName: "SingleJiandaoSe2",
              GunNameBurst: "SingleJiandaoSe2",
              DebugLevel: 0,
              Revealable: false,
              IsPooled: true,
              FindInBattle: true,
              HideMesuem: false,
              IsUpgradable: true,
              Rarity: Rarity.Rare,
              Type: CardType.Skill,
              TargetType: TargetType.SingleEnemy,
              Colors: new List<ManaColor>() { ManaColor.Black },
              IsXCost: false,
              Cost: new ManaGroup() { Any = 4 },
              UpgradedCost: null,
              MoneyCost: null,
              Damage: 0,
              UpgradedDamage: null,
              Block: null,
              UpgradedBlock: null,
              Shield: null,
              UpgradedShield: null,
              Value1: 4,
              UpgradedValue1: 16,
              Value2: 4,
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

              RelativeEffects: new List<string>() { "DisarmStatus" },
              UpgradedRelativeEffects: new List<string>() { "DisarmStatus" },
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
    [EntityLogic(typeof(BoundaryOfDeathDef))]
    public sealed class BoundaryOfDeath : RuinaCard
    {
        int dmgCap = 256;
        public int? EnemyDamage { get; set; }
        public string EnemyDamageStr { get => EnemyDamage != null ? EnemyDamage.ToString() : "none"; }
        public override string Description
        {
            get
            {
                _ = RuntimeFormatterExtensions.RuntimeFormat("{Damage}", this.FormatWrapper);
                return base.Description;
            }
        }
        protected override void OnEnterBattle(BattleController battle)
        {
            this.HandleBattleEvent(battle.Player.DamageDealing, (DamageDealingEventArgs args) =>
            {
                if (args.Targets != null)
                {
                    EnemyDamage = null;

                    foreach (EnemyUnit enemy in args.Targets.Select(e => e as EnemyUnit).Where(e => e != null))
                    {
                        foreach (Intention intention in enemy.Intentions)
                        {
                            if (intention is AttackIntention attackIntention)
                            {
                                EnemyDamage = attackIntention.CalculateDamage(attackIntention.Damage);
                                break;
                            }
                            else if (intention is SpellCardIntention spellCardIntention && spellCardIntention.Damage != null)
                            {
                                EnemyDamage = spellCardIntention.CalculateDamage(spellCardIntention.Damage.Value);
                                break;
                            }
                        }
                    }
                }
            });
        }
        Condition conditions;

        public bool HpCondition
        {
            get
            {
                return ((base.Battle.Player.IsAlive) && (base.Battle.Player.Hp % 4 == 0 || base.Battle.Player.Hp <= base.Battle.Player.MaxHp / 4));
            }
        }
        public int CardPlayCount
        {
            get
            {
                return base.Battle.TurnCardUsageHistory.Count + 1;
            }
        }
        public bool CardPlayCondition
        {
            get
            {
                return base.Battle != null && CardPlayCount == 4;
            }
        }
        public bool HandPositionCondition
        {
            get
            {
                return base.Battle != null && base.Battle.HandZone.Count >= 4 && (this == this.Battle.HandZone[3] || this == this.Battle.HandZone[^4]);
            }
        }
        public bool DrawDiscardPileCondition
        {
            get
            {
                return base.Battle != null && (base.Battle.DrawZone.Count % 4 == 0 || base.Battle.DiscardZone.Count % 4 == 0);
            }
        }
        public int CalculateDamage
        {
            get
            {
                int dmg = base.Value1;
                foreach (Condition condition in Enum.GetValues(typeof(Condition)))
                {
                    if (condition != Condition.None && this.conditions.HasFlag(condition))
                    {
                        dmg *= base.Value2;
                    }
                }

                dmg = Math.Min(dmg, this.dmgCap);
                return dmg;
            }
        }

        [Flags]
        public enum Condition
        {
            None = 0,
            HpCondition = 1 << 0,
            CardPlayCondition = 1 << 1,
            HandPositionCondition = 1 << 2,
            DrawDiscardPileCondition = 1 << 3
        }
        public Condition evalConditions()
        {
            Condition conditions = Condition.None;
            if (this.HpCondition)
            {
                conditions |= Condition.HpCondition;
            }
            if (this.CardPlayCondition)
            {
                conditions |= Condition.CardPlayCondition;
            }
            if (this.HandPositionCondition)
            {
                conditions |= Condition.HandPositionCondition;
            }
            if (this.DrawDiscardPileCondition) 
            {
                conditions |= Condition.DrawDiscardPileCondition;
            }
            return conditions;
        }
        public override bool Triggered
        {
            get
            {
                this.conditions = this.evalConditions();
                return this.conditions > Condition.None;
            }
        }
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return AttackAction(selector.SelectedEnemy, DamageInfo.Reaction((float)this.CalculateDamage, false), base.GunName);

            bool disarm = false;

            foreach (Intention intention in selector.SelectedEnemy.Intentions)
            {
                if (intention is AttackIntention attackIntention)
                {
                    if (attackIntention.CalculateDamage(attackIntention.Damage) < this.CalculateDamage)
                    {
                        disarm = true;
                        break;
                    }
                }
                else if (intention is SpellCardIntention spellCardIntention)
                {
                    DamageInfo spellCardDmgInfo = spellCardIntention.Damage ?? new DamageInfo(0, DamageType.Attack);
                    if (spellCardIntention.CalculateDamage(spellCardDmgInfo) < this.CalculateDamage)
                    {
                        disarm = true;
                        break;
                    }
                }
            }
            if (selector.SelectedEnemy.IsAlive && disarm)
            {
                yield return DebuffAction<DisarmStatus>(selector.SelectedEnemy);
            }
            this.conditions &= Condition.None;
            yield break;    
        }
    }
}

