namespace Shin_Megami_Tensei_Model.Data.Components;

public class Battle(Unit atkUnit, Unit defUnit, int roundNumber)
{
    public readonly Unit AtkUnit = atkUnit;
    public readonly Unit DefUnit = defUnit;
    public readonly int RoundNumber = roundNumber;
}