namespace Shin_Megami_Tensei_Model.CombatSystem.Rules
{
    public class ValidationContext
    {
        public HashSet<string> SeenUnits { get; set; }
        public int SamuraiCount { get; set; }

        public ValidationContext(HashSet<string> seenUnits)
        {
            SeenUnits = seenUnits;
            SamuraiCount = 0;
        }
    }
}
