namespace Shin_Megami_Tensei_Model.CombatSystem.Core
{
    public interface IActionHandler
    {
        bool Execute(ActionContext actionContext);
    }
}
