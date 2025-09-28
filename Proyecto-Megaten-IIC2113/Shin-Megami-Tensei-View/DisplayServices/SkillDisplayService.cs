using System.Collections.Generic;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_View.ConsoleLib
{
    public class SkillDisplayService
    {
        private const int MinimumChoice = 1;
        private const int InvalidChoice = -1;
        private const string Separator = "----------------------------------------";
        private const string SkillSelectionFormat = "Seleccione una habilidad para que {0} use";
        private const string SkillOptionFormat = "{0}-{1} MP:{2}";
        private const string CancelOptionFormat = "{0}-Cancelar";

        private readonly View view;

        public SkillDisplayService(View view)
        {
            this.view = view;
        }

        public void ShowSkillSelection(UnitInstanceContext unit, List<Skill> availableSkills)
        {
            ShowSeparator();
            ShowSkillSelectionHeader(unit.Name);
            ShowSkillOptions(availableSkills);
            ShowCancelOption(availableSkills.Count);
        }

        private void ShowSeparator()
        {
            view.WriteLine(Separator);
        }

        private void ShowSkillSelectionHeader(string unitName)
        {
            view.WriteLine(string.Format(SkillSelectionFormat, unitName));
        }

        private void ShowSkillOptions(List<Skill> availableSkills)
        {
            for (int i = 0; i < availableSkills.Count; i++)
            {
                ShowSkillOption(i + 1, availableSkills[i]);
            }
        }

        private void ShowSkillOption(int index, Skill skill)
        {
            view.WriteLine(string.Format(SkillOptionFormat, index, skill.Name, skill.Cost));
        }

        private void ShowCancelOption(int skillCount)
        {
            view.WriteLine(string.Format(CancelOptionFormat, skillCount + 1));
        }

        public int GetSkillChoice(int maxSkills)
        {
            return ReadChoice(maxSkills + 1);
        }

        private int ReadChoice(int maxChoice)
        {
            var input = ReadInput();
            return TryValidateChoice(input, maxChoice);
        }

        private string ReadInput()
        {
            return view.ReadLine();
        }

        private int TryValidateChoice(string input, int maxChoice)
        {
            var choice = ParseChoiceOrDefault(input);
            if (choice == InvalidChoice)
                return InvalidChoice;
            if (!IsChoiceWithinRange(choice, maxChoice))
                return InvalidChoice;
            return choice;
        }

        private int ParseChoiceOrDefault(string input)
        {
            return int.TryParse(input, out int choice) ? choice : InvalidChoice;
        }

        private bool IsChoiceWithinRange(int choice, int maxChoice)
        {
            return choice >= MinimumChoice && choice <= maxChoice;
        }
    }
}


