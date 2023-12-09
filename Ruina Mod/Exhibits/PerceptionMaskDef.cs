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
using MonoMod.Utils;

namespace Ruina_Mod.Exhibits
{
    public sealed class PerceptionMaskDef : ExhibitTemplate
    {
        public override IdContainer GetId()
        {
            return nameof(PerceptionMask);
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
                Value1: 1,
                Value2: null,
                Value3: null,
                Mana: new ManaGroup() { Colorless = 1 },
                BaseManaRequirement: null,
                BaseManaColor: ManaColor.Colorless,
                BaseManaAmount: 1,
                HasCounter: false,
                InitialCounter: null,
                Keywords: Keyword.None,
                RelativeEffects: new List<string>() { "Graze" },
                RelativeCards: new List<string>() { }
                )
            {

            };
            return exhibitConfig;
        }
    }

    [EntityLogic(typeof(PerceptionMaskDef))]
    public sealed class PerceptionMask : ShiningExhibit
    {
        protected override void OnEnterBattle()
        {
            base.ReactBattleEvent<CardUsingEventArgs>(base.Battle.CardUsed, new EventSequencedReactor<CardUsingEventArgs>(this.OnCardUsed));
            base.ReactBattleEvent<UnitEventArgs>(base.Battle.Player.TurnStarted, new EventSequencedReactor<UnitEventArgs>(this.OnPlayerTurnStarted));
            base.HandleBattleEvent<DamageDealingEventArgs>(base.Owner.DamageDealing, new GameEventHandler<DamageDealingEventArgs>(this.OnDamageDealing));
        }

        protected override void OnLeaveBattle()
        {
            base.Counter = 0;
        }

        private IEnumerable<BattleAction> OnPlayerTurnStarted(GameEventArgs args)
        {
            base.NotifyActivating();
            yield return new ApplyStatusEffectAction<Graze>(base.Owner, new int?(base.Value1), null, null, null, 0f, true);
            base.Counter = 0;
            yield break;
        }
        private void OnDamageDealing(DamageDealingEventArgs args)
        {
            if (args.DamageInfo.DamageType == DamageType.Attack && base.Counter == 0)
            {
                base.NotifyActivating();
                args.DamageInfo = new DamageInfo(args.DamageInfo.Damage, DamageType.HpLose, isGrazed:args.DamageInfo.IsGrazed, isAccuracy:args.DamageInfo.IsAccuracy, dontBreakPerfect:args.DamageInfo.DontBreakPerfect);
                args.AddModifier(this);
            }
        }

        private IEnumerable<BattleAction> OnCardUsed(CardUsingEventArgs args)
        {
            if (base.Owner.IsInTurn && args.Card.CardType == CardType.Attack && base.Counter == 0)
            {
                base.NotifyActivating();
                base.Counter++;
            }
            yield break;
        }
    }
}

