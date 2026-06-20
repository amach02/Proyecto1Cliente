using System.Text.Json.Serialization;

namespace Proyecto1Cliente.Models.Domain
{
    public class PlantaHospedadora
    {
        [JsonPropertyName("id_planta")]
        public int IdPlanta { get; set; }

        [JsonPropertyName("nombre_cientifico")]
        public string? NombreCientifico { get; set; }

        [JsonPropertyName("nombre_comun")]
        public string? NombreComun { get; set; }

        public string NombreCompleto => string.IsNullOrWhiteSpace(NombreComun)
            ? NombreCientifico
            : $"{NombreCientifico} ({NombreComun})";
    }
}