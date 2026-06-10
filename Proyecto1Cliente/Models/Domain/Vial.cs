using System.Text.Json.Serialization;

namespace Proyecto1Cliente.Models.Domain
{
    public class Vial
    {
        [JsonPropertyName("id_vial")]
        public int IdVial { get; set; }

        [JsonPropertyName("vial")]
        public string Codigo { get; set; }

        [JsonPropertyName("caja")]
        public string Caja { get; set; }
    }
}