using Shin_Megami_Tensei_Model.Domain.States;

namespace Shin_Megami_Tensei
{
    public class BattleContext
    {
        public BattleState BattleState { get; set; }
        public string Player1Name { get; set; }
        public string Player2Name { get; set; }
    }
}
