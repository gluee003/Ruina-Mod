using LBoL.Core.Cards;


namespace Ruina_Mod
{
    public abstract class RuinaCard : Card
    {
        public enum AttackType
        {
            Slash,
            Pierce,
            Blunt
        }
        public AttackType attackType;
    }
}