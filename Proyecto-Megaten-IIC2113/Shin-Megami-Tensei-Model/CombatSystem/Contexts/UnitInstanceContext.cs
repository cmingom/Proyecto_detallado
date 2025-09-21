using System;
using System.Collections.Generic;

namespace Shin_Megami_Tensei_Model.Domain.Entities
{
    public class UnitInstanceContext
    {
        public string Name { get; init; } = string.Empty;
        public int MaxHP { get; init; }
        public int HP { get; set; }
        public int MaxMP { get; init; }
        public int MP { get; set; }
        public int Str { get; init; }
        public int Skl { get; init; }
        public int Mag { get; init; }
        public int Spd { get; init; }
        public bool IsSamurai { get; init; }
        public char Position { get; init; }
        public List<string> Skills { get; init; } = new List<string>();
        public Dictionary<string, string> Affinities { get; }
        public int OriginalOrder { get; }

        public UnitInstanceContext(
            string name,
            int maxHP,
            int maxMP,
            int str,
            int skl,
            int mag,
            int spd,
            bool isSamurai,
            char position,
            int originalOrder,
            List<string>? skills = null,
            Dictionary<string, string>? affinities = null)
        {
            Name = name;
            MaxHP = maxHP;
            HP = maxHP;
            MaxMP = maxMP;
            MP = maxMP;
            Str = str;
            Skl = skl;
            Mag = mag;
            Spd = spd;
            IsSamurai = isSamurai;
            Position = position;
            Skills = skills ?? new List<string>();
            Affinities = affinities != null
                ? new Dictionary<string, string>(affinities, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            OriginalOrder = originalOrder;
        }

        public void OnTurnStart()
        {
            // Lógica que se ejecuta al inicio del turno de la unidad
            // Por ahora este método está vacío, pero puede expandirse en el futuro
        }
    }
}
