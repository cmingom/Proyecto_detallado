namespace Shin_Megami_Tensei_Model.Domain.Entities;

public sealed class Samurai : Unit
{
    public Samurai() : base() { }

    public override List<string> GetAvailableActions()
    {
        return new List<string>
        {
            "Atacar",
            "Disparar",
            "Usar Habilidad",
            "Invocar",
            "Pasar Turno",
            "Rendirse"
        };
    }
}
