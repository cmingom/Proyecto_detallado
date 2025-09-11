using Shin_Megami_Tensei_Model.CombatSystem.Contexts;

namespace Shin_Megami_Tensei_Model.CombatSystem.Interfaces
{
    public interface IActionExecutor
    {
        bool CanProcessSelectedAction(ActionProcessingContext context);
    }
}
