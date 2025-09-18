using System.Text.Json.Serialization;

namespace Shin_Megami_Tensei_Model.Domain.Entities;

public sealed class Monster : Unit
{
    public Monster() : base() { }

    public override List<string> GetAvailableActions()
    {
        return new List<string>
        {
            "Atacar",
            "Usar Habilidad",
            "Invocar",
            "Pasar Turno"
        };
    }
}
