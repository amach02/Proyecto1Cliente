using System.Text.Json.Serialization;

namespace Proyecto1Cliente.Models.Domain
{
    public class Fotografia
    {
        [JsonPropertyName("id_fotografia")]
        public int IdFotografia { get; set; }

        [JsonPropertyName("ruta")]
        public string Ruta { get; set; }

        [JsonPropertyName("id_especimen")]
        public int IdEspecimen { get; set; }
    }
}