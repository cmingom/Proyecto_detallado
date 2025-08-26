using System.Text.Json;
using Shin_Megami_Tensei_Model.Exceptions.TeamFileValidation;

namespace Shin_Megami_Tensei_Model.SetUp.Loader;

public static class UnitInfoLoader
{
    private static string _basePath = "";

    public static void SetBasePath(string basePath)
    {
        _basePath = basePath;
    }

    public static UnitInfo GetFromTextLine(string textLine)
    {
        var unitsInfo = LoadAllUnitsInfo();
        var name = GetUnitNameFromLine(textLine);
        var isSamurai = textLine.StartsWith("[Samurai]");
        return FindUnitInfoWithName(unitsInfo, name, isSamurai);
    }

    private static List<UnitInfo> LoadAllUnitsInfo()
    {
        var allUnits = new List<UnitInfo>();
        
        var samuraiPath = Path.Combine(_basePath, "samurai.json");
        var samuraiJson = File.ReadAllText(samuraiPath);
        var samurais = JsonSerializer.Deserialize<UnitInfo[]>(samuraiJson) ?? throw new UnitInfoLoadException();
        foreach (var samurai in samurais)
        {
            samurai.IsSamurai = true;
            samurai.Skills = new List<string>();
        }
        allUnits.AddRange(samurais);
        
        var monstersPath = Path.Combine(_basePath, "monsters.json");
        var monstersJson = File.ReadAllText(monstersPath);
        var monsters = JsonSerializer.Deserialize<UnitInfo[]>(monstersJson) ?? throw new UnitInfoLoadException();
        foreach (var monster in monsters)
        {
            monster.IsSamurai = false;
        }
        allUnits.AddRange(monsters);
        
        return allUnits;
    }

    private static UnitInfo FindUnitInfoWithName(List<UnitInfo> unitsInfo, string name, bool isSamurai)
    {
        foreach (var unitInfo in unitsInfo)
            if (unitInfo.Name == name && unitInfo.IsSamurai == isSamurai)
                return unitInfo;
        throw new UnitInfoNotFoundException();
    }

    private static string GetUnitNameFromLine(string info)
    {
        if (info.StartsWith("[Samurai]"))
        {
            info = info.Replace("[Samurai]", "").Trim();
        }
        
        if (!info.Contains('(')) return info;
        var startIndex = info.IndexOf('(');
        return info[..startIndex].Trim();
    }
}
