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
    }
}