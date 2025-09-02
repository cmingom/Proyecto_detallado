namespace Shin_Megami_Tensei_Model.CombatSystem.Contexts
{
    public class PlayerInfo
    {
        public string Name { get; }
        public string Number { get; }

        public PlayerInfo(string name, string number)
        {
            Name = name;
            Number = number;
        }
    }
}
