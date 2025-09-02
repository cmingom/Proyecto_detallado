using System;

namespace Shin_Megami_Tensei_Model.CombatSystem.Contexts
{
    public class TeamValidatorConfig
    {
        public Func<string, bool> UnitExists { get; }
        public Func<string, bool> SkillExists { get; }

        public TeamValidatorConfig(Func<string, bool> unitExists, Func<string, bool> skillExists)
        {
            UnitExists = unitExists;
            SkillExists = skillExists;
        }
    }
}
