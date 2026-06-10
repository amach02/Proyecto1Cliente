using System.Text.Json.Serialization;

namespace Proyecto1Cliente.Models.Domain
{
    public class Gaveta
    {
        [JsonPropertyName("id_gaveta")]
        public int IdGaveta { get; set; }

        [JsonPropertyName("codigo")]
        public string Codigo { get; set; }
    }
}