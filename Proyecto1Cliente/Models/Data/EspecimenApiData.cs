using Proyecto1Cliente.Models.Domain;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Proyecto1Cliente.Models.Data
{
    public class EspecimenApiData
    {
        private readonly HttpClient _http;

        // ¡Solución global! Aquí le decimos a .NET que acepte los números que vengan entre comillas desde PHP
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        public EspecimenApiData(HttpClient http) => _http = http;

        // GET: Listar todos
        public async Task<IEnumerable<Especimen>> ListarAsync()
        {
            var resp = await _http.GetAsync("");
            if (!resp.IsSuccessStatusCode) return new List<Especimen>();

            var json = await resp.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(json);
            var dataElement = document.RootElement.GetProperty("data");

            return JsonSerializer.Deserialize<List<Especimen>>(dataElement.GetRawText(), _jsonOptions) ?? new List<Especimen>();
        }

        // GET: Obtener uno solo por ID
        public async Task<Especimen?> ObtenerAsync(int id)
        {
            var resp = await _http.GetAsync($"?id={id}");
            if (!resp.IsSuccessStatusCode) return null;

            var json = await resp.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(json);
            var dataElement = document.RootElement.GetProperty("data");

            return JsonSerializer.Deserialize<Especimen>(dataElement.GetRawText(), _jsonOptions);
        }

        // POST: Registrar
        public async Task<bool> RegistrarAsync(Especimen esp)
        {
            // Ajustado para que envíe exactamente lo que el SW.php de PHP está esperando
            var payload = new
            {
                codigo_id = esp.CodigoId,
                localizacion_recoleccion = esp.LocalizacionRecoleccion,
                fecha_recoleccion = esp.FechaRecoleccion,
                estado = esp.Estado ?? "activo",
                id_especie = esp.IdEspecie,
                id_gaveta = esp.IdGaveta,
                id_vial = esp.IdVial,
                id_usuario = 1
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync("", content);

            return resp.IsSuccessStatusCode;
        }

        // PUT: Editar
        public async Task<bool> ActualizarAsync(Especimen esp)
        {
            // Ajustado para coincidir con el método editarEspecimen de PHP
            var payload = new
            {
                id_especimen = esp.IdEspecimen,
                codigo_id = esp.CodigoId,
                id_gaveta = esp.IdGaveta,
                id_vial = esp.IdVial,
                id_usuario = 1
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var req = new HttpRequestMessage(HttpMethod.Put, "") { Content = content };
            var resp = await _http.SendAsync(req);

            return resp.IsSuccessStatusCode;
        }

        // DELETE: Inhabilitar (Soft Delete)
        public async Task<bool> InhabilitarAsync(int id)
        {
            var resp = await _http.DeleteAsync($"?id={id}");
            return resp.IsSuccessStatusCode;
        }

        public async Task<List<Especie>> ObtenerEspeciesAsync()
        {
            var resp = await _http.GetAsync("?catalogo=especies");
            if (!resp.IsSuccessStatusCode) return new List<Especie>();

            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return JsonSerializer.Deserialize<List<Especie>>(doc.RootElement.GetProperty("data").GetRawText(), _jsonOptions) ?? new();
        }

        public async Task<List<Gaveta>> ObtenerGavetasAsync()
        {
            var resp = await _http.GetAsync("?catalogo=gavetas");
            if (!resp.IsSuccessStatusCode) return new List<Gaveta>();

            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return JsonSerializer.Deserialize<List<Gaveta>>(doc.RootElement.GetProperty("data").GetRawText(), _jsonOptions) ?? new();
        }

        public async Task<List<Vial>> ObtenerVialesAsync()
        {
            var resp = await _http.GetAsync("?catalogo=viales");
            if (!resp.IsSuccessStatusCode) return new List<Vial>();

            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return JsonSerializer.Deserialize<List<Vial>>(doc.RootElement.GetProperty("data").GetRawText(), _jsonOptions) ?? new();
        }
    }
}