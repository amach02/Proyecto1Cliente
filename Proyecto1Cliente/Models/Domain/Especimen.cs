using System.Text.Json.Serialization;

namespace Proyecto1Cliente.Models.Domain
{
    public class Especimen
    {
        [JsonPropertyName("id_especimen")]
        public int IdEspecimen { get; set; }

        [JsonPropertyName("codigo_id")]
        public string? CodigoId { get; set; }

        [JsonPropertyName("localizacion_recoleccion")]
        public string? LocalizacionRecoleccion { get; set; }

        [JsonPropertyName("fecha_recoleccion")]
        public string? FechaRecoleccion { get; set; }

        [JsonPropertyName("estado")]
        public string? Estado { get; set; }

        [JsonPropertyName("id_especie")]
        public int? IdEspecie { get; set; }

        [JsonPropertyName("id_gaveta")]
        public int? IdGaveta { get; set; }

        [JsonPropertyName("id_vial")]
        public int? IdVial { get; set; }

        // Recibe el arreglo de fotos que nos mandará SW.php
        [JsonPropertyName("fotografias")]
        public List<Fotografia>? Fotografias { get; set; }
    }
}