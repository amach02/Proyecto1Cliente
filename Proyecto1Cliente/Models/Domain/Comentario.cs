using System.Text.Json.Serialization;

namespace Proyecto1Cliente.Models.Domain
{
    public class Comentario
    {
        [JsonPropertyName("id_comentario")]
        public int IdComentario { get; set; }

        [JsonPropertyName("id_especimen")]
        public int IdEspecimen { get; set; }

        [JsonPropertyName("id_usuario")]
        public int IdUsuario { get; set; }

        [JsonPropertyName("nombre_autor")]
        public string? NombreAutor { get; set; }

        [JsonPropertyName("comentario")]
        public string Texto { get; set; }

        [JsonPropertyName("fecha_hora")]
        public string? FechaHoraString { get; set; }

        [JsonIgnore]
        public DateTime FechaHora => DateTime.TryParse(FechaHoraString, out var fechaConvertida) ? fechaConvertida : DateTime.Now;
    }
}