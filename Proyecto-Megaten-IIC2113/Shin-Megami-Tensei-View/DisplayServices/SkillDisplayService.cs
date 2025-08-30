using System;
using System.Collections.Generic;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_View.ConsoleLib
{
    public class SkillDisplayService
    {
        private readonly View view;

        public SkillDisplayService(View view)
        {
            this.view = view;
        }

        public void ShowSkillSelection(UnitInstance unit, List<Skill> availableSkills)
        {
            view.WriteLine("----------------------------------------");
            view.WriteLine($"Seleccione una habilidad para que {unit.Name} use");
            
            for (int i = 0; i < availableSkills.Count; i++)
            {
                var skill = availableSkills[i];
                view.WriteLine($"{i + 1}-{skill.Name} MP:{skill.Cost}");
            }
            view.WriteLine($"{availableSkills.Count + 1}-Cancelar");
        }

        public int GetSkillChoice(int maxSkills)
        {
            var input = view.ReadLine();
            if (!int.TryParse(input, out int choice) || choice < 1 || choice > maxSkills + 1)
                return -1;
            return choice;
        }
    }
}
