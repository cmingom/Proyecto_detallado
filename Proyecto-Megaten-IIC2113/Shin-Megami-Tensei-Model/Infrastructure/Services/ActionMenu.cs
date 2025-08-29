using System.Collections.Generic;

namespace Shin_Megami_Tensei_Model.Infrastructure.Services
{
    /// <summary>
    /// Proporciona opciones textuales para el usuario sobre acciones y objetivos durante el turno de una unidad.
    /// Separar esta lógica del motor de juego permite que la vista
    /// renderice menús sin tener conocimiento de los objetos de dominio subyacentes.
    /// </summary>
    public class ActionMenu
    {
        /// <summary>
        /// Estructura de datos mínima que describe un objetivo potencial. El Game o
        /// TurnEngine pueden construir instancias de este record desde sus propios
        /// objetos de dominio sin exponer esos objetos al menú.
        /// </summary>
        public record TargetInfo(string Name, int HP, int MaxHP, int MP, int MaxMP);

        /// <summary>
        /// Obtiene la lista de acciones disponibles para una unidad durante su turno. El orden de
        /// las acciones debe permanecer estable porque los tests esperan que la numeración
        /// coincida con las opciones mostradas en pantalla.
        /// </summary>
        public IReadOnlyList<string> GetActions()
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

        /// <summary>
        /// Construye una lista de etiquetas de objetivos para que el usuario seleccione al elegir
        /// un objetivo. Cada enemigo está numerado comenzando en 1 y la opción final
        /// corresponde a "Cancelar". El llamador es responsable de mapear el
        /// índice seleccionado de vuelta a la instancia de enemigo subyacente.
        /// </summary>
        /// <param name="enemies">Los enemigos vivos para listar como posibles objetivos.</param>
        /// <returns>Una lista de strings adecuada para mostrar por la Vista.</returns>
        public IReadOnlyList<string> GetTargetOptions(IEnumerable<TargetInfo> enemies)
        {
            var options = new List<string>();
            int i = 1;
            foreach (var enemy in enemies)
            {
                options.Add($"{i}-{enemy.Name} HP:{enemy.HP}/{enemy.MaxHP} MP:{enemy.MP}/{enemy.MaxMP}");
                i++;
            }
            options.Add($"{i}-Cancelar");
            return options;
        }
    }
}