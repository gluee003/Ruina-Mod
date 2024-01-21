using LBoL.Core.Cards;


namespace Ruina_Mod
{
    public abstract class RuinaCard : Card
    {
        public enum AttackType
        {
            None,
            Slash,
            Pierce,
            Blunt
        }
        public virtual AttackType attackType{ get; }
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
    }
}