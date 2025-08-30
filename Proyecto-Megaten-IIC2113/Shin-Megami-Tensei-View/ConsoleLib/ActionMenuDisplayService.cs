using System;
using System.Collections.Generic;
using Shin_Megami_Tensei_Model.Domain.Entities;

namespace Shin_Megami_Tensei_View.ConsoleLib
{
    public class ActionMenuDisplayService
    {
        private readonly View view;

        public ActionMenuDisplayService(View view)
        {
            this.view = view;
        }

        public void ShowActionMenu(UnitInstance actingUnit, List<string> actions)
        {
            view.WriteLine("----------------------------------------");
            view.WriteLine($"Seleccione una acción para {actingUnit.Name}");
            
            for (int i = 0; i < actions.Count; i++)
            {
                view.WriteLine($"{i + 1}: {actions[i]}");
            }
        }

        public int GetActionChoice(int maxActions)
        {
            var input = view.ReadLine();
            if (!int.TryParse(input, out int choice) || choice < 1 || choice > maxActions)
                return -1;
            return choice;
        }

        public void ShowTargetSelection(UnitInstance attacker, List<UnitInstance> targets)
        {
            view.WriteLine("----------------------------------------");
            view.WriteLine($"Seleccione un objetivo para {attacker.Name}");
            
            for (int i = 0; i < targets.Count; i++)
            {
                var currentTarget = targets[i];
                view.WriteLine($"{i + 1}-{currentTarget.Name} HP:{currentTarget.HP}/{currentTarget.MaxHP} MP:{currentTarget.MP}/{currentTarget.MaxMP}");
            }
            view.WriteLine($"{targets.Count + 1}-Cancelar");
        }

        public int GetTargetChoice(int maxTargets)
        {
            var input = view.ReadLine();
            if (!int.TryParse(input, out int choice) || choice < 1 || choice > maxTargets + 1)
                return -1;
            return choice;
        }

        public void ShowAttackResult(UnitInstance attacker, UnitInstance target, int damage, bool isGunAttack)
        {
            view.WriteLine("----------------------------------------");
            
            string attackType = isGunAttack ? "dispara a" : "ataca a";
            view.WriteLine($"{attacker.Name} {attackType} {target.Name}");
            view.WriteLine($"{target.Name} recibe {damage} de daño");
            view.WriteLine($"{target.Name} termina con HP:{target.HP}/{target.MaxHP}");
        }
    }
}
