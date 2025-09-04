using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;
using Shin_Megami_Tensei_Model.CombatSystem.Enums;

namespace Shin_Megami_Tensei_View.ConsoleLib
{
    public class ActionMenuDisplayService
    {
        private const int MINIMUM_CHOICE = 1;
        private const int INVALID_CHOICE = -1;
        private const string SEPARATOR = "----------------------------------------";
        private const string ACTION_SELECTION_FORMAT = "Seleccione una acción para {0}";
        private const string TARGET_SELECTION_FORMAT = "Seleccione un objetivo para {0}";
        private const string ACTION_OPTION_FORMAT = "{0}: {1}";
        private const string TARGET_OPTION_FORMAT = "{0}-{1} HP:{2}/{3} MP:{4}/{5}";
        private const string CANCEL_OPTION_FORMAT = "{0}-Cancelar";
        private const string ATTACK_RESULT_FORMAT = "{0} {1} {2}";
        private const string DAMAGE_RESULT_FORMAT = "{0} recibe {1} de daño";
        private const string HP_RESULT_FORMAT = "{0} termina con HP:{1}/{2}";
        private const string GUN_ATTACK_TEXT = "dispara a";
        private const string PHYSICAL_ATTACK_TEXT = "ataca a";

        private readonly View view;

        public ActionMenuDisplayService(View view)
        {
            this.view = view;
        }

        public void ShowActionMenu(GetUnitInstance actingGetUnit, List<string> actions)
        {
            ShowSeparator();
            ShowActionSelectionHeader(actingGetUnit.Name);
            ShowActionOptions(actions);
        }

        private void ShowSeparator()
        {
            view.WriteLine(SEPARATOR);
        }

        private void ShowActionSelectionHeader(string unitName)
        {
            view.WriteLine(string.Format(ACTION_SELECTION_FORMAT, unitName));
        }

        private void ShowActionOptions(List<string> actions)
        {
            for (int i = 0; i < actions.Count; i++)
            {
                ShowActionOption(i + 1, actions[i]);
            }
        }

        private void ShowActionOption(int index, string action)
        {
            view.WriteLine(string.Format(ACTION_OPTION_FORMAT, index, action));
        }

        public int GetActionChoice(int maxActions)
        {
            return GetValidatedChoice(maxActions);
        }

        public void ShowTargetSelection(GetUnitInstance attacker, List<GetUnitInstance> targets)
        {
            ShowSeparator();
            ShowTargetSelectionHeader(attacker.Name);
            ShowTargetOptions(targets);
            ShowCancelOption(targets.Count);
        }

        private void ShowTargetSelectionHeader(string attackerName)
        {
            view.WriteLine(string.Format(TARGET_SELECTION_FORMAT, attackerName));
        }

        private void ShowTargetOptions(List<GetUnitInstance> targets)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                ShowTargetOption(i + 1, targets[i]);
            }
        }

        private void ShowTargetOption(int index, GetUnitInstance target)
        {
            view.WriteLine(string.Format(TARGET_OPTION_FORMAT, index, target.Name, target.HP, target.MaxHP, target.MP, target.MaxMP));
        }

        private void ShowCancelOption(int targetCount)
        {
            view.WriteLine(string.Format(CANCEL_OPTION_FORMAT, targetCount + 1));
        }

        public int GetTargetChoice(int maxTargets)
        {
            return GetValidatedChoice(maxTargets + 1);
        }

        private int GetValidatedChoice(int maxChoice)
        {
            var input = view.ReadLine();
            if (!IsValidChoice(input, maxChoice, out int choice))
                return INVALID_CHOICE;
            return choice;
        }

        private bool IsValidChoice(string input, int maxChoice, out int choice)
        {
            choice = 0;
            return int.TryParse(input, out choice) && 
                   choice >= MINIMUM_CHOICE && 
                   choice <= maxChoice;
        }

        public void ShowAttackResult(AttackResultContext context)
        {
            ShowSeparator();
            ShowAttackAction(context.Attacker.Name, context.Target.Name, context.AttackType);
            ShowDamageResult(context.Target.Name, context.Damage);
            ShowHpResult(context.Target.Name, context.Target.HP, context.Target.MaxHP);
        }

        private bool IsGunAttack(AttackType attackType)
        {
            return attackType == AttackType.Gun;
        }

        private void ShowAttackAction(string attackerName, string targetName, AttackType attackType)
        {
            string attackTypeText = GetAttackTypeText(attackType);
            view.WriteLine(string.Format(ATTACK_RESULT_FORMAT, attackerName, attackTypeText, targetName));
        }

        private string GetAttackTypeText(AttackType attackType)
        {
            return IsGunAttack(attackType) ? GUN_ATTACK_TEXT : PHYSICAL_ATTACK_TEXT;
        }

        private void ShowDamageResult(string targetName, int damage)
        {
            view.WriteLine(string.Format(DAMAGE_RESULT_FORMAT, targetName, damage));
        }

        private void ShowHpResult(string targetName, int currentHp, int maxHp)
        {
            view.WriteLine(string.Format(HP_RESULT_FORMAT, targetName, currentHp, maxHp));
        }
    }
}
