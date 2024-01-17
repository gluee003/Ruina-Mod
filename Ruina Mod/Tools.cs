using LBoL.Base;
using LBoL.Core.Cards;
using LBoL.Core.Randoms;
using LBoL.Core;
using LBoL.Presentation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ruina_Mod
{
    public abstract class Tools
    {
        public static Card[] RollCardsAnyFilter(RandomGen rng, CardWeightTable weightTable, int count, ManaGroup? manaLimit = null, bool colorLimit = false, bool applyFactors = false, bool ensureCount = false, Predicate<Card> filter = null)
        {
            var gr = GameMaster.Instance?.CurrentGameRun;
            if (gr == null)
                throw new InvalidOperationException("Rolling cards when run is not started.");

            var innitialPool = gr.CreateValidCardsPool(weightTable, manaLimit, colorLimit, applyFactors, null);

            var filteredPool = new UniqueRandomPool<Card>();

            foreach (var e in innitialPool)
            {
                var card = Library.CreateCard(e.Elem);
                if (filter(card))
                {
                    card.GameRun = gr;
                    filteredPool.Add(card, e.Weight);
                }
            }

            return filteredPool.SampleMany(rng, count, ensureCount);
        }
    }
}
