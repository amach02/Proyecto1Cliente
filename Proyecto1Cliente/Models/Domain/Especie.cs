using System.Text.Json.Serialization;

namespace Proyecto1Cliente.Models.Domain
{
    public class Especie
    {
        [JsonPropertyName("id_especie")]
        public int IdEspecie { get; set; }

        [JsonPropertyName("especie")]
        public string EspecieNombre { get; set; }

        [JsonPropertyName("genero")]
        public string Genero { get; set; }

        [JsonPropertyName("familia")]
        public string Familia { get; set; }

        [JsonPropertyName("orden")]
        public string Orden { get; set; }
    }
}