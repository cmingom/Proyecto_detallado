namespace Shin_Megami_Tensei_Model.CombatSystem.Contexts
{
    public class TeamHeaderResult
    {
        public bool IsTeamHeader { get; }
        public bool IsTeam1 { get; }

        public TeamHeaderResult(bool isTeamHeader, bool isTeam1)
        {
            IsTeamHeader = isTeamHeader;
            IsTeam1 = isTeam1;
        }
    }
}
