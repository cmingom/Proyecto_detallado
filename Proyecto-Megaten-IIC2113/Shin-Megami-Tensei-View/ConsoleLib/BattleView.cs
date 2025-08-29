using System;
using System.Collections.Generic;
using System.Linq;
using Shin_Megami_Tensei_Model.Domain.States;
using Shin_Megami_Tensei_Model.Domain.Entities;
using Shin_Megami_Tensei_Model.CombatSystem.Core;

namespace Shin_Megami_Tensei_View.ConsoleLib
{
    public class BattleView : IBattleView
    {
        private readonly View view;

        public BattleView(View view)
        {
            this.view = view;
        }

        public void ShowRoundHeader(string playerName, string playerNumber)
        {
            view.WriteLine("----------------------------------------");
            view.WriteLine($"Ronda de {playerName} ({playerNumber})");
        }

        public void ShowBattlefield(BattleState battleState, string player1Name, string player2Name)
        {
            view.WriteLine("----------------------------------------");
            ShowTeamStatus(battleState.Team1, player1Name, "J1");
            ShowTeamStatus(battleState.Team2, player2Name, "J2");
        }

        private void ShowTeamStatus(TeamState team, string playerName, string playerNumber)
        {
            view.WriteLine($"Equipo de {playerName} ({playerNumber})");
            
            char[] positions = { 'A', 'B', 'C', 'D' };
            
            for (int i = 0; i < 4; i++)
            {
                var unit = team.Units[i];
                if (unit != null)
                {
                    if (unit.IsSamurai || unit.HP > 0)
                    {
                        view.WriteLine($"{positions[i]}-{unit.Name} HP:{unit.HP}/{unit.MaxHP} MP:{unit.MP}/{unit.MaxMP}");
                    }
                    else
                    {
                        view.WriteLine($"{positions[i]}-");
                    }
                }
                else
                {
                    view.WriteLine($"{positions[i]}-");
                }
            }
        }

        public void ShowTurnCounters(BattleState battleState)
        {
            view.WriteLine("----------------------------------------");
            view.WriteLine($"Full Turns: {battleState.FullTurns}");
            view.WriteLine($"Blinking Turns: {battleState.BlinkingTurns}");
        }

        public void ShowActionOrderBySpeed(List<UnitInstance> actionOrder)
        {
            view.WriteLine("----------------------------------------");
            view.WriteLine("Orden:");
            
            for (int i = 0; i < actionOrder.Count; i++)
            {
                view.WriteLine($"{i + 1}-{actionOrder[i].Name}");
            }
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

        public void ShowSurrender(string playerName, string playerNumber, string winnerName, string winnerNumber)
        {
            view.WriteLine("----------------------------------------");
            view.WriteLine($"{playerName} ({playerNumber}) se rinde");
            view.WriteLine("----------------------------------------");
            view.WriteLine($"Ganador: {winnerName} ({winnerNumber})");
        }

        public void ShowTurnConsumption()
        {
            view.WriteLine("----------------------------------------");
            view.WriteLine("Se han consumido 1 Full Turn(s) y 0 Blinking Turn(s)");
            view.WriteLine("Se han obtenido 0 Blinking Turn(s)");
        }

        public void ShowWinner(string winnerName, string winnerNumber)
        {
            view.WriteLine("----------------------------------------");
            view.WriteLine($"Ganador: {winnerName} ({winnerNumber})");
        }
    }
}
