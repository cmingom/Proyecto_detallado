using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Shin_Megami_Tensei_Model.Domain.Interfaces;

namespace Shin_Megami_Tensei_Model.Infrastructure.Repositories
{
    public class DataRepository : IUnitRepo, ISkillRepo
    {
        private readonly HashSet<string> _unitNames = new(StringComparer.Ordinal);
        private readonly HashSet<string> _skillNames = new(StringComparer.Ordinal);

        public DataRepository(string dataFolder)
        {
            if (dataFolder == null) throw new ArgumentNullException(nameof(dataFolder));
            try
            {
                LoadUnits(Path.Combine(dataFolder, "samurai.json"));
                LoadUnits(Path.Combine(dataFolder, "monsters.json"));
                LoadSkills(Path.Combine(dataFolder, "skills.json"));
            }
            catch
            {
            }
        }

        private void LoadUnits(string filePath)
        {
            if (!File.Exists(filePath)) return;
            try
            {
                var json = File.ReadAllText(filePath);
                var list = JsonSerializer.Deserialize<List<UnitDTO>>(json) ?? new List<UnitDTO>();
                foreach (var u in list)
                {
                    if (!string.IsNullOrWhiteSpace(u.name))
                        _unitNames.Add(u.name);
                }
            }
            catch
            {
            }
        }

        private void LoadSkills(string filePath)
        {
            if (!File.Exists(filePath)) return;
            try
            {
                var json = File.ReadAllText(filePath);
                var list = JsonSerializer.Deserialize<List<SkillDTO>>(json) ?? new List<SkillDTO>();
                foreach (var s in list)
                {
                    if (!string.IsNullOrWhiteSpace(s.name))
                        _skillNames.Add(s.name);
                }
            }
            catch
            {
            }
        }

        bool IUnitRepo.Contains(string unitName) =>
            unitName != null && _unitNames.Contains(unitName);

        bool ISkillRepo.Contains(string skillName) =>
            skillName != null && _skillNames.Contains(skillName);

        private class UnitDTO { public string? name { get; set; } }
        private class SkillDTO { public string? name { get; set; } }
    }
}