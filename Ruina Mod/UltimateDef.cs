using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Battle.Interactions;
using LBoL.Core.Cards;
using LBoL.Core.Randoms;
using LBoL.Core.Units;
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
using static Ruina_Mod.RolandPlayerDef;
using static Ruina_Mod.Tools;

namespace Ruina_Mod
{
    public sealed class RolandUltDef : UltimateSkillTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(RolandUlt);
        }

        public override LocalizationOption LoadLocalization()
        {
            var loc = new GlobalLocalization(embeddedSource);
            loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "UltimateSkillEn.yaml");
            return loc;
        }

        public override Sprite LoadSprite()
        {
            return ResourceLoader.LoadSprite("EGO.png", embeddedSource);
        }

        public override UltimateSkillConfig MakeConfig()
        {
            var config = new UltimateSkillConfig(
                Id: "",
                Order: 10,
                PowerCost: 50,
                PowerPerLevel: 100,
                MaxPowerLevel: 2,
                RepeatableType: UsRepeatableType.OncePerTurn,
                Damage: 0,
                Value1: 5,
                Value2: 0,
                Keywords: Keyword.None,
                RelativeEffects: new List<string>() { },
                RelativeCards: new List<string>() { }
                );

            return config;
        }
    }
    [EntityLogic(typeof(RolandUltDef))]
    public sealed class RolandUlt : UltimateSkill
    {
        public RolandUlt()
        {
            base.TargetType = LBoL.Base.TargetType.Nobody;
            base.GunName = "Simple1";
        }
        protected override IEnumerable<BattleAction> Actions(UnitSelector selector)
        {
            yield return PerformAction.Spell(Battle.Player, "RolandUlt");
            Card[] array = Tools.RollCardsAnyFilter(GameRun.BattleCardRng, new CardWeightTable(RarityWeightTable.BattleCard, OwnerWeightTable.AllOnes, CardTypeWeightTable.CanBeLoot), base.Value1, null, false, false, false, false, 
                           (Card card) => card is RuinaCard ruinacard && ruinacard.pageType == RuinaCard.PageType.EGO);

            MiniSelectCardInteraction interaction = new MiniSelectCardInteraction(array, false, false, false);
            yield return new InteractionAction(interaction, false);

            Card selectedCard = interaction.SelectedCard;
            yield return new AddCardsToHandAction(new Card[] { selectedCard });
            yield break;
        }
    }
}
