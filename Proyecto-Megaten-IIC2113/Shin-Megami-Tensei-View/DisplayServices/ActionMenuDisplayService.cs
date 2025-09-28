using System.Collections.Generic;

using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Contexts;
using Shin_Megami_Tensei_Model.CombatSystem.Enums;

namespace Shin_Megami_Tensei_View.ConsoleLib
{
    public class ActionMenuDisplayService
    {
        private const int MinimumChoice = 1;
        private const int InvalidChoice = -1;
        private const string Separator = "----------------------------------------";
        private const string ActionSelectionFormat = "Seleccione una acción para {0}";
        private const string TargetSelectionFormat = "Seleccione un objetivo para {0}";
        private const string ActionOptionFormat = "{0}: {1}";
        private const string TargetOptionFormat = "{0}-{1} HP:{2}/{3} MP:{4}/{5}";
        private const string CancelOptionFormat = "{0}-Cancelar";
        private const string AttackResultFormat = "{0} {1} {2}";
        private const string DamageResultFormat = "{0} recibe {1} de daño";
        private const string HpResultFormat = "{0} termina con HP:{1}/{2}";
        private const string GunAttackText = "dispara a";
        private const string PhysicalAttackText = "ataca a";
        private const string FireAttackText = "lanza fuego a";
        private const string IceAttackText = "lanza hielo a";
        private const string ElecAttackText = "lanza electricidad a";
        private const string ForceAttackText = "lanza viento a";
        private const string SummonPositionHeader = "Seleccione una posición para invocar";
        private const string EmptySlotText = "Vacío";
        private const string ResistMessageFormat = "{0} es resistente el ataque de {1}";
        private const string WeakMessageFormat = "{0} es débil contra el ataque de {1}";
        private const string BlockMessageFormat = "{0} bloquea el ataque de {1}";
        private const string AbsorbMessageFormat = "{0} absorbe {1} daño";
        private const string RepelMessageFormat = "{0} devuelve {1} daño a {2}";

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
            view.WriteLine(Separator);
        }

        private void ShowActionSelectionHeader(string unitName)
        {
            view.WriteLine(string.Format(ActionSelectionFormat, unitName));
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
            view.WriteLine(string.Format(ActionOptionFormat, index, action));
        }

        public int GetActionChoice(int maxActions)
        {
            return ReadChoice(maxActions);
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
            view.WriteLine(string.Format(TargetSelectionFormat, attackerName));
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
            view.WriteLine(string.Format(TargetOptionFormat, index, target.Name, target.HP, target.MaxHP, target.MP, target.MaxMP));
        }

        private void ShowCancelOption(int targetCount)
        {
            view.WriteLine(string.Format(CancelOptionFormat, targetCount + 1));
        }

        public int GetTargetChoice(int maxTargets)
        {
            return ReadChoice(maxTargets + 1);
        }

        private int ReadChoice(int maxChoice)
        {
            var input = view.ReadLine();
            if (!TryParseChoice(input, maxChoice, out int choice))
            {
                return InvalidChoice;
            }

            return choice;
        }

        private bool TryParseChoice(string input, int maxChoice, out int choice)
        {
            choice = 0;
            return int.TryParse(input, out choice) &&
                   choice >= MinimumChoice &&
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
            view.WriteLine(string.Format(AttackResultFormat, context.Attacker.Name, verb, context.Target.Name));
        }

        private static string GetAttackVerb(DamageElement element)
        {
            return element switch
            {
                DamageElement.Gun => GunAttackText,
                DamageElement.Fire => FireAttackText,
                DamageElement.Ice => IceAttackText,
                DamageElement.Elec => ElecAttackText,
                DamageElement.Force => ForceAttackText,
                _ => PhysicalAttackText
            };
        }

        private void ShowAffinityReaction(AttackResultContext context)
        {
            switch (context.Reaction)
            {
                case AffinityReaction.Weak:
                    view.WriteLine(string.Format(WeakMessageFormat, context.Target.Name, context.Attacker.Name));
                    break;
                case AffinityReaction.Resist:
                    view.WriteLine(string.Format(ResistMessageFormat, context.Target.Name, context.Attacker.Name));
                    break;
                case AffinityReaction.Null:
                    view.WriteLine(string.Format(BlockMessageFormat, context.Target.Name, context.Attacker.Name));
                    break;
                case AffinityReaction.Repel:
                    view.WriteLine(string.Format(RepelMessageFormat, context.Target.Name, context.DamageToAttacker, context.Attacker.Name));
                    break;
                case AffinityReaction.Drain:
                    view.WriteLine(string.Format(AbsorbMessageFormat, context.Target.Name, context.DamageToTarget));
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
            view.WriteLine(string.Format(DamageResultFormat, targetName, damage));
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
            view.WriteLine(string.Format(HpResultFormat, targetName, currentHp, maxHp));
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
            return ReadChoice(maxOptions);
        }

        public void ShowSummonPositionMenu(List<(char Slot, UnitInstanceContext? Unit)> positionOptions)
        {
            ShowSeparator();
            view.WriteLine(SummonPositionHeader);

            for (int i = 0; i < positionOptions.Count; i++)
            {
                var (slot, unit) = positionOptions[i];
                var description = unit == null
                    ? EmptySlotText
                    : $"{unit.Name} HP:{unit.HP}/{unit.MaxHP} MP:{unit.MP}/{unit.MaxMP}";
                var positionNumber = GetPositionNumber(slot);
                view.WriteLine($"{i + 1}-{description} (Puesto {positionNumber})");
            }

            view.WriteLine(string.Format(CancelOptionFormat, positionOptions.Count + 1));
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
            return ReadChoice(maxOptions);
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





