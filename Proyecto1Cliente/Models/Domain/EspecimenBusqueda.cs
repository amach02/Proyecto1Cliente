using System.Text.Json.Serialization;

namespace Proyecto1Cliente.Models.Domain
{
    public class EspecimenBusqueda
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

        // Taxonomía
        [JsonPropertyName("especie")]
        public string? Especie { get; set; }

        [JsonPropertyName("genero")]
        public string? Genero { get; set; }

        [JsonPropertyName("familia")]
        public string? Familia { get; set; }

        [JsonPropertyName("orden")]
        public string? Orden { get; set; }

        // Ubicación física
        [JsonPropertyName("gaveta")]
        public string? Gaveta { get; set; }

        [JsonPropertyName("gabinete")]
        public string? Gabinete { get; set; }

        [JsonPropertyName("vial")]
        public string? Vial { get; set; }

        [JsonPropertyName("caja")]
        public string? Caja { get; set; }

        // Propiedad calculada para mostrar ubicación amigable en la vista
        public string UbicacionFisica
        {
            get
            {
                if (!string.IsNullOrEmpty(Gaveta))
                    return $"Gabinete {Gabinete} → Gaveta {Gaveta}";
                if (!string.IsNullOrEmpty(Vial))
                    return $"Caja {Caja} → Vial {Vial}";
                return "Sin ubicación asignada";
            }
        }
    }
}
