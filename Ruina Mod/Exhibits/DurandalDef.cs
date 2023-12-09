using LBoL.ConfigData;
using LBoL.Core.Cards;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Attributes;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using System;
using System.Collections.Generic;
using System.Text;
using static Ruina_Mod.BepinexPlugin;
using UnityEngine;
using LBoL.Core;
using LBoL.Base;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Base.Extensions;
using System.Collections;
using LBoL.EntityLib.Cards.Neutral.Blue;
using LBoL.EntityLib.Exhibits.Shining;
using LBoL.EntityLib.Exhibits;
using LBoL.Core.Units;
using LBoL.Core.StatusEffects;

namespace Ruina_Mod.Exhibits
{
    public sealed class DurandalDef : ExhibitTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(Durandal);
        }

        public override LocalizationOption LoadLocalization()
        {
            var loc = new GlobalLocalization(embeddedSource);
            loc.LocalizationFiles.AddLocaleFile(LBoL.Core.Locale.En, "ExhibitsEn.yaml");
            return loc;
        }

        public override ExhibitSprites LoadSprite()
        {
            var folder = "Resources.";
            var exhibitSprites = new ExhibitSprites();
            Func<string, Sprite> wrap = (s) => ResourceLoader.LoadSprite((folder + GetId() + s + ".png"), embeddedSource);

            exhibitSprites.main = wrap("");

            return exhibitSprites;
        }

        public override ExhibitConfig MakeConfig()
        {
            var exhibitConfig = new ExhibitConfig(
                Index: 0,
                Id: "",
                Order: 10,
                IsDebug: false,
                IsPooled: false,
                IsSentinel: false,
                Revealable: false,
                Appearance: AppearanceType.Nowhere,
                Owner: null,
                LosableType: ExhibitLosableType.DebutLosable,
                Rarity: Rarity.Shining,
                Value1: 2,
                Value2: 3,
                Value3: null,
                Mana: new ManaGroup() { Colorless = 1 },
                BaseManaRequirement: null,
                BaseManaColor: ManaColor.Colorless,
                BaseManaAmount: 1,
                HasCounter: true,
                InitialCounter: null,
                Keywords: Keyword.None,
                RelativeEffects: new List<string>() { "Firepower", "Spirit" },
                RelativeCards: new List<string>() { }
                )
            {

            };
            return exhibitConfig;
        }
    }

    [EntityLogic(typeof(DurandalDef))]
    public sealed class Durandal : ShiningExhibit
    {
        protected override void OnEnterBattle()
        {
            base.ReactBattleEvent<CardUsingEventArgs>(base.Battle.CardUsed, new EventSequencedReactor<CardUsingEventArgs>(this.OnCardUsed));
        }
        protected override void OnLeaveBattle()
        {
            base.Counter = 0;
        }
        private IEnumerable<BattleAction> OnCardUsed(CardUsingEventArgs args)
        {
            if (base.Owner.IsInTurn)
            {
                base.Counter = (base.Counter + 1) % (base.Value1 + 1);
                if (base.Counter == 2)
                // gain Firepower and Spirit after playing 2 cards
                {
                    base.NotifyActivating();
                    yield return new ApplyStatusEffectAction<Firepower>(base.Owner, new int?(base.Value2), null, null, null, 0f, true);
                    yield return new ApplyStatusEffectAction<Spirit>(base.Owner, new int?(base.Value2), null, null, null, 0f, true);
                }
                if (base.Counter == 0)
                // remove Firepower and Spirit gained after playing 3rd card
                {
                    base.NotifyActivating();
                    yield return new ApplyStatusEffectAction<FirepowerNegative>(base.Owner, new int?(base.Value2), null, null, null, 0f, true);
                    yield return new ApplyStatusEffectAction<SpiritNegative>(base.Owner, new int?(base.Value2), null, null, null, 0f, true);
                }
            }
            yield break;
        }
    }
}
