using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto1Cliente.Models.Data;
using Proyecto1Cliente.Models.Domain;
using System.Security.Claims;

namespace Proyecto1Cliente.Controllers
{
    [Authorize]
    public class EspecimenApiController : Controller
    {
        private readonly EspecimenApiData _apiData;

        public EspecimenApiController(EspecimenApiData apiData)
        {
            _apiData = apiData;
        }

        // GET: Listar con paginación y búsqueda rápida
        // GET: Listar con paginación solamente
        public async Task<IActionResult> Index(int pagina = 1)
        {
            // 1. Traemos la lista completa desde tu API en PHP
            var especimenes = await _apiData.ListarAsync();

            // 2. Configuración de la paginación
            int registrosPorPagina = 5; // Puedes cambiar este número para mostrar 10 o 15 por página
            int totalRegistros = especimenes.Count();
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            // Evitamos rangos inválidos en la paginación
            if (pagina < 1) pagina = 1;
            if (pagina > totalPaginas && totalPaginas > 0) pagina = totalPaginas;

            // 3. Cortamos la lista para mostrar solo el bloque de la página actual
            var listaPaginada = especimenes
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToList();

            // 4. Enviamos los datos de control a la Vista mediante ViewBag
            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            return View(listaPaginada);
        }

        public async Task<IActionResult> Details(int id)
        {
            var esp = await _apiData.ObtenerAsync(id);
            if (esp == null) return NotFound();
            return View(esp);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Especies = await _apiData.ObtenerEspeciesAsync();
            ViewBag.Gavetas = await _apiData.ObtenerGavetasAsync();
            ViewBag.Viales = await _apiData.ObtenerVialesAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Especimen esp)
        {
            if (!ModelState.IsValid) return View(esp);

            await _apiData.RegistrarAsync(esp);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var esp = await _apiData.ObtenerAsync(id);
            if (esp == null) return NotFound();
            return View(esp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Especimen esp)
        {
            // Ahora esto no dará error de compilación porque ambos son enteros
            if (id != esp.IdEspecimen || !ModelState.IsValid) return View(esp);

            await _apiData.ActualizarAsync(esp);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var esp = await _apiData.ObtenerAsync(id);
            if (esp == null) return NotFound();
            return View(esp);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiData.InhabilitarAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Vista de comentarios
        public async Task<IActionResult> Comentarios(int id)
        {
            var esp = await _apiData.ObtenerAsync(id);
            if (esp == null) return NotFound();

            // Pasamos el espécimen y sus comentarios a la vista
            ViewBag.Especimen = esp;
            var comentarios = await _apiData.ObtenerComentariosAsync(id);

            return View(comentarios);
        }

        // POST: Guardar comentario desde AJAX
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> GuardarComentario([FromBody] Comentario com)
        {
            if (string.IsNullOrWhiteSpace(com.Texto))
            {
                return BadRequest("El comentario no puede estar vacío.");
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdClaim, out int realUserId))
            {
                com.IdUsuario = realUserId;
            }
            else
            {
                return Unauthorized();
            }

            // Capturamos el string de error
            var errorMsg = await _apiData.RegistrarComentarioAsync(com);

            // Si está vacío, fue un éxito rotundo
            if (string.IsNullOrEmpty(errorMsg)) return Ok();

            // Si trae texto, mandamos el código 500 acompañado del error real de PHP
            return StatusCode(500, errorMsg);
        }

        // GET: Muestra el formulario vacío
        public IActionResult Buscar()
        {
            return View(new List<EspecimenBusqueda>());
        }

        // GET: Ejecuta la búsqueda con el criterio
        [HttpGet]
        public async Task<IActionResult> BuscarResultados(string criterio)
        {
            if (string.IsNullOrWhiteSpace(criterio))
                return RedirectToAction(nameof(Buscar));

            var resultados = await _apiData.BuscarAsync(criterio);
            ViewBag.Criterio = criterio;
            return View("Buscar", resultados);
        }

        // GET: Mostrar pantalla de gestión de plantas para un espécimen
        public async Task<IActionResult> Plantas(int id)
        {
            var esp = await _apiData.ObtenerAsync(id);
            if (esp == null) return NotFound();

            ViewBag.Especimen = esp;
            ViewBag.CatalogoPlantas = await _apiData.ObtenerTodasPlantasAsync(); // Catálogo de plantas (Criterio 1)

            var plantasVinculadas = await _apiData.ObtenerPlantasPorEspecimenAsync(id);
            return View(plantasVinculadas);
        }

        // POST: Procesar la asociación (Criterio 2)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VincularPlanta(int idEspecimen, int idPlanta)
        {
            if (idPlanta <= 0) return RedirectToAction(nameof(Plantas), new { id = idEspecimen });

            var exito = await _apiData.VincularPlantaAsync(idEspecimen, idPlanta);
            if (!exito)
                TempData["Error"] = "No se pudo vincular la planta hospedadora (puede que ya esté asociada).";
            else
                TempData["Exito"] = "Planta hospedadora vinculada correctamente.";

            return RedirectToAction(nameof(Plantas), new { id = idEspecimen });
        }

        // POST: Procesar la desvinculación (Criterio 3)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DesvincularPlanta(int idEspecimen, int idPlanta)
        {
            var exito = await _apiData.DesvincularPlantaAsync(idEspecimen, idPlanta);
            if (!exito)
                TempData["Error"] = "Ocurrió un error al intentar desvincular la planta.";
            else
                TempData["Exito"] = "La relación se eliminó correctamente.";

            return RedirectToAction(nameof(Plantas), new { id = idEspecimen });
        }
    }
}