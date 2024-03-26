using LBoL.Base.Extensions;
using LBoL.Core;
using LBoL.Core.Cards;
using System;


namespace Ruina_Mod
{
    public abstract class RuinaCard : Card
    {
        [Flags]
        public enum AttackType
        {
            None = 0b000,
            Slash = 0b001,
            Pierce = 0b010,
            Blunt = 0b100
        }
        public virtual AttackType attackType{ get; }
        public string attack_type
        {
            get { return attackType.ToString().Replace(",", "") != "None" ? attackType.ToString().Replace(",", "") : ""; }
        }
        public enum PageType
        {
            Normal,
            Abnormality,
            EGO
        }
        public virtual PageType pageType { get; }
        public enum PageRange
        {
            None,
            Melee,
            Range
        }
        public virtual PageRange pageRange { get;  }
        public string page_range
        {
            get { return pageRange.ToString() != "None" ? pageRange.ToString() : ""; }
        }
        public override string Description
        {
            get
            {
                if (!page_range.IsNullOrEmpty() || !attack_type.IsNullOrEmpty())
                {
                    return StringDecorator.Decorate(base.Description + $"\n|{page_range} {attack_type}|");
                }
                else
                {
                    return base.Description;
                }
            }
        }
    }
}