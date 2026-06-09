using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Proyecto1Cliente.Models.Data
{
    public class UsuarioLoginResponse
    {
        [JsonPropertyName("id_usuario")]
        public int IdUsuario { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("nombre_rol")]
        public string NombreRol { get; set; }
    }

    public class UsuarioApiData
    {
        private readonly HttpClient _http;

        // ¡LA SOLUCIÓN! Le decimos a .NET que acepte números aunque vengan entre comillas desde PHP
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        public UsuarioApiData(HttpClient http) => _http = http;

        public async Task<UsuarioLoginResponse?> LoginAsync(string correo, string contrasena)
        {
            var payload = new { accion = "login", correo, contrasena };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var resp = await _http.PostAsync("", content);

            if (!resp.IsSuccessStatusCode) return null;

            var json = await resp.Content.ReadAsStringAsync();

            // El escudo por si PHP vuelve a enviar advertencias en el futuro
            int startIndex = json.IndexOf('{');
            if (startIndex >= 0)
            {
                json = json.Substring(startIndex);
            }

            try
            {
                using var doc = JsonDocument.Parse(json);
                var dataElement = doc.RootElement.GetProperty("data");
                return JsonSerializer.Deserialize<UsuarioLoginResponse>(dataElement.GetRawText(), _jsonOptions);
            }
            catch (JsonException ex)
            {
                // Si llegara a fallar, ahora te dirá exactamente el motivo del error de .NET
                throw new Exception($"Error de formato al leer el JSON: {ex.Message} | JSON Recibido: {json}");
            }
        }
    }
}