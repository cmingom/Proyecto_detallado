using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class PassTurnProcessor : IActionHandler
    {
        public bool Execute(ActionContext actionContext)
        {
            return true; // Pasar turno siempre se completa
        }
    }
}
