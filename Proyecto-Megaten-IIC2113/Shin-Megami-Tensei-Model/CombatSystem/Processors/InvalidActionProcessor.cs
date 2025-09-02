using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public class InvalidActionProcessor : IActionHandler
    {
        public bool Execute(ActionContext actionContext)
        {
            return false; // Acción inválida nunca se completa
        }
    }
}
