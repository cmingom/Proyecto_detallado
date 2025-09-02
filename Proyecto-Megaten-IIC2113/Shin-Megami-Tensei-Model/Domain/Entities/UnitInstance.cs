using System;
using System.Collections.Generic;

namespace Shin_Megami_Tensei_Model.Domain.Entities
{
    public class UnitInstance
    {
        private const int MINIMUM_HP = 0;
        
        public string Name { get; init; } = string.Empty;
        public int MaxHP { get; init; }
        public int HP { get; set; }
        public int MaxMP { get; init; }
        public int MP { get; set; }
        public int Str { get; init; }
        public int Skl { get; init; }
        public int Spd { get; init; }
        public bool IsSamurai { get; init; }
        public char Position { get; init; }
        public List<string> Skills { get; init; } = new();

        public UnitInstance(string name, int maxHP, int maxMP,
            int str, int skl, int spd,
            bool isSamurai, char position, List<string>? skills = null)
        {
            Name = name;
            MaxHP = maxHP;
            HP = maxHP;
            MaxMP = maxMP;
            MP = maxMP;
            Str = str;
            Skl = skl;
            Spd = spd;
            IsSamurai = isSamurai;
            Position = position;
            Skills = skills ?? new();
        }
        
        public bool IsAlive => HP > MINIMUM_HP;
    }
}