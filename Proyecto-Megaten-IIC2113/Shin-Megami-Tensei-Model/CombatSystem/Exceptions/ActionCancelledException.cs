namespace Shin_Megami_Tensei_Model.CombatSystem.Exceptions
{
    public class ActionCancelledException : Exception
    {
        public ActionCancelledException() : base("Action was cancelled by user")
        {
        }
    }
}
