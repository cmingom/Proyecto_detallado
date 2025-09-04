using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_View.ConsoleLib
{
    public class SkillDisplayService
    {
        private const int MINIMUM_CHOICE = 1;
        private const int INVALID_CHOICE = -1;
        private const string SEPARATOR = "----------------------------------------";
        private const string SKILL_SELECTION_FORMAT = "Seleccione una habilidad para que {0} use";
        private const string SKILL_OPTION_FORMAT = "{0}-{1} MP:{2}";
        private const string CANCEL_OPTION_FORMAT = "{0}-Cancelar";

        private readonly View view;

        public SkillDisplayService(View view)
        {
            this.view = view;
        }

        public void ShowSkillSelection(GetUnitInstance getUnit, List<Skill> availableSkills)
        {
            ShowSeparator();
            ShowSkillSelectionHeader(getUnit.Name);
            ShowSkillOptions(availableSkills);
            ShowCancelOption(availableSkills.Count);
        }

        private void ShowSeparator()
        {
            view.WriteLine(SEPARATOR);
        }

        private void ShowSkillSelectionHeader(string unitName)
        {
            view.WriteLine(string.Format(SKILL_SELECTION_FORMAT, unitName));
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
            view.WriteLine(string.Format(SKILL_OPTION_FORMAT, index, skill.Name, skill.Cost));
        }

        private void ShowCancelOption(int skillCount)
        {
            view.WriteLine(string.Format(CANCEL_OPTION_FORMAT, skillCount + 1));
        }

        public int GetSkillChoice(int maxSkills)
        {
            return GetValidatedChoice(maxSkills + 1);
        }

        private int GetValidatedChoice(int maxChoice)
        {
            var input = GetUserInput();
            return ValidateUserChoice(input, maxChoice);
        }

        private string GetUserInput()
        {
            return view.ReadLine();
        }

        private int ValidateUserChoice(string input, int maxChoice)
        {
            var choice = ParseChoice(input);
            if (choice == INVALID_CHOICE)
                return INVALID_CHOICE;
            if (!IsValidChoiceRange(choice, maxChoice))
                return INVALID_CHOICE;
            return choice;
        }

        private int ParseChoice(string input)
        {
            return int.TryParse(input, out int choice) ? choice : INVALID_CHOICE;
        }

        private bool IsValidChoiceRange(int choice, int maxChoice)
        {
            return choice >= MINIMUM_CHOICE && choice <= maxChoice;
        }
    }
}
