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
        private const string FIRE_ATTACK_TEXT = "lanza fuego a";
        private const string ICE_ATTACK_TEXT = "lanza hielo a";
        private const string ELEC_ATTACK_TEXT = "lanza electricidad a";
        private const string FORCE_ATTACK_TEXT = "lanza viento a";
        private const string SUMMON_POSITION_HEADER = "Seleccione una posición para invocar";
        private const string EMPTY_SLOT_TEXT = "Vacío";
        private const string RESIST_MESSAGE_FORMAT = "{0} es resistente el ataque de {1}";
        private const string WEAK_MESSAGE_FORMAT = "{0} es débil contra el ataque de {1}";
        private const string BLOCK_MESSAGE_FORMAT = "{0} bloquea el ataque de {1}";
        private const string ABSORB_MESSAGE_FORMAT = "{0} absorbe {1} daño";
        private const string REPEL_MESSAGE_FORMAT = "{0} devuelve {1} daño a {2}";

        private readonly View view;

        public ActionMenuDisplayService(View view)
        {
            this.view = view;
        }

        public void ShowActionMenu(UnitInstanceContext actingUnit, List<string> actions)
        {
            ShowSeparator();
            ShowActionSelectionHeader(actingUnit.Name);
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

        public void ShowTargetSelection(UnitInstanceContext attacker, List<UnitInstanceContext> targets)
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

        private void ShowTargetOptions(List<UnitInstanceContext> targets)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                ShowTargetOption(i + 1, targets[i]);
            }
        }

        private void ShowTargetOption(int index, UnitInstanceContext target)
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
            {
                return INVALID_CHOICE;
            }

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
            if (context.IsFirstHit)
            {
                ShowSeparator();
            }

            ShowAttackAction(context);
            ShowAffinityReaction(context);

            if (ShouldShowDamageLine(context.Reaction))
            {
                ShowDamageResult(context.Target.Name, context.DamageToTarget);
            }

            if (context.IsFinalHit)
            {
                ShowHpAfterAttack(context);
            }
        }

        private void ShowAttackAction(AttackResultContext context)
        {
            var verb = GetAttackVerb(context.Element);
            view.WriteLine(string.Format(ATTACK_RESULT_FORMAT, context.Attacker.Name, verb, context.Target.Name));
        }

        private static string GetAttackVerb(DamageElement element)
        {
            return element switch
            {
                DamageElement.Gun => GUN_ATTACK_TEXT,
                DamageElement.Fire => FIRE_ATTACK_TEXT,
                DamageElement.Ice => ICE_ATTACK_TEXT,
                DamageElement.Elec => ELEC_ATTACK_TEXT,
                DamageElement.Force => FORCE_ATTACK_TEXT,
                _ => PHYSICAL_ATTACK_TEXT
            };
        }

        private void ShowAffinityReaction(AttackResultContext context)
        {
            switch (context.Reaction)
            {
                case AffinityReaction.Weak:
                    view.WriteLine(string.Format(WEAK_MESSAGE_FORMAT, context.Target.Name, context.Attacker.Name));
                    break;
                case AffinityReaction.Resist:
                    view.WriteLine(string.Format(RESIST_MESSAGE_FORMAT, context.Target.Name, context.Attacker.Name));
                    break;
                case AffinityReaction.Null:
                    view.WriteLine(string.Format(BLOCK_MESSAGE_FORMAT, context.Target.Name, context.Attacker.Name));
                    break;
                case AffinityReaction.Repel:
                    view.WriteLine(string.Format(REPEL_MESSAGE_FORMAT, context.Target.Name, context.DamageToAttacker, context.Attacker.Name));
                    break;
                case AffinityReaction.Drain:
                    view.WriteLine(string.Format(ABSORB_MESSAGE_FORMAT, context.Target.Name, context.DamageToTarget));
                    break;
            }
        }

        private static bool ShouldShowDamageLine(AffinityReaction reaction)
        {
            return reaction == AffinityReaction.Neutral ||
                   reaction == AffinityReaction.Resist ||
                   reaction == AffinityReaction.Weak;
        }

        private void ShowDamageResult(string targetName, int damage)
        {
            view.WriteLine(string.Format(DAMAGE_RESULT_FORMAT, targetName, damage));
        }

        private void ShowHpAfterAttack(AttackResultContext context)
        {
            if (context.Reaction == AffinityReaction.Repel)
            {
                ShowHpResult(context.Attacker.Name, context.AttackerHpAfter, context.Attacker.MaxHP);
                return;
            }

            ShowHpResult(context.Target.Name, context.TargetHpAfter, context.Target.MaxHP);
        }

        private void ShowHpResult(string targetName, int currentHp, int maxHp)
        {
            view.WriteLine(string.Format(HP_RESULT_FORMAT, targetName, currentHp, maxHp));
        }

        public void ShowSummonMenu(List<UnitInstanceContext> availableUnits)
        {
            ShowSeparator();
            view.WriteLine("Seleccione un monstruo para invocar");

            for (int i = 0; i < availableUnits.Count; i++)
            {
                var unit = availableUnits[i];
                view.WriteLine($"{i + 1}-{unit.Name} HP:{unit.HP}/{unit.MaxHP} MP:{unit.MP}/{unit.MaxMP}");
            }

            view.WriteLine($"{availableUnits.Count + 1}-Cancelar");
        }

        public int GetSummonChoice(int maxOptions)
        {
            return GetValidatedChoice(maxOptions);
        }

        public void ShowSummonPositionMenu(List<(char Slot, UnitInstanceContext? Unit)> positionOptions)
        {
            ShowSeparator();
            view.WriteLine(SUMMON_POSITION_HEADER);

            for (int i = 0; i < positionOptions.Count; i++)
            {
                var (slot, unit) = positionOptions[i];
                var description = unit == null
                    ? EMPTY_SLOT_TEXT
                    : $"{unit.Name} HP:{unit.HP}/{unit.MaxHP} MP:{unit.MP}/{unit.MaxMP}";
                var positionNumber = GetPositionNumber(slot);
                view.WriteLine($"{i + 1}-{description} (Puesto {positionNumber})");
            }

            view.WriteLine(string.Format(CANCEL_OPTION_FORMAT, positionOptions.Count + 1));
        }

        private static int GetPositionNumber(char slot)
        {
            return slot switch
            {
                'A' => 1,
                'B' => 2,
                'C' => 3,
                'D' => 4,
                _ => 0
            };
        }

        public int GetSummonPositionChoice(int maxOptions)
        {
            return GetValidatedChoice(maxOptions);
        }

        public void ShowGuardAction(UnitInstanceContext unit)
        {
            ShowSeparator();
            view.WriteLine($"{unit.Name} se defiende");
        }

        public void ShowHealSuccess(UnitInstanceContext unit, UnitInstanceContext target, string skillName, int healAmount)
        {
            ShowSeparator();
            view.WriteLine($"{unit.Name} cura a {target.Name}");
            view.WriteLine($"{target.Name} recibe {healAmount} de HP");
            view.WriteLine($"{target.Name} termina con HP:{target.HP}/{target.MaxHP}");
        }

        public void ShowHealFailure(UnitInstanceContext unit, UnitInstanceContext target, string skillName)
        {
            ShowSeparator();
            view.WriteLine($"{unit.Name} usa {skillName} en {target.Name}");
            view.WriteLine($"{target.Name} no puede ser curado");
            view.WriteLine($"{target.Name} termina con HP:{target.HP}/{target.MaxHP}");
        }

        public void ShowReviveResult(UnitInstanceContext unit, UnitInstanceContext target, string skillName, int revivedHp, bool showSeparator = true)
        {
            if (showSeparator)
            {
                ShowSeparator();
            }

            view.WriteLine($"{unit.Name} revive a {target.Name}");
            view.WriteLine($"{target.Name} recibe {revivedHp} de HP");
            view.WriteLine($"{target.Name} termina con HP:{target.HP}/{target.MaxHP}");
        }

        public void ShowSkillUsage(UnitInstanceContext unit, string skillName)
        {
            ShowSeparator();
            view.WriteLine($"{unit.Name} usa {skillName}");
        }
    }
}


