using Shin_Megami_Tensei_Model.Data.Components;
using Shin_Megami_Tensei_Model.Exceptions.TeamFileValidation;

namespace Shin_Megami_Tensei_Model.SetUp.Validator;

public static class TeamsValidator
{
    public static void ValidateTeams(Team team1, Team team2)
    {
        TeamIsValid(team1);
        TeamIsValid(team2);
    }

    private static void TeamIsValid(Team team)
    {
        ValidateHasSamurai(team);
        ValidateOnlyOneSamurai(team);
        ValidateCorrectUnitsNumber(team);
        ValidateUnitsAreUnique(team);
    }

    private static void ValidateHasSamurai(Team team)
    {
        if (!team.HasSamurai())
            throw new TeamHasNoSamuraiException();
    }

    private static void ValidateOnlyOneSamurai(Team team)
    {
        if (team.SamuraiCount() > 1)
            throw new TeamHasMultipleSamuraisException();
    }

    private static void ValidateCorrectUnitsNumber(Team team)
    {
        if (team.Count > 8)
            throw new IncorrectTeamUnitsNumberException();
    }

    private static void ValidateUnitsAreUnique(Team team)
    {
        var unitsNames = team.Select(unit => unit.Name);
        var uniqueUnitsNames = unitsNames.Distinct();
        if (uniqueUnitsNames.Count() != team.Count)
            throw new TeamUnitsAreNotUniqueException();
    }
}
