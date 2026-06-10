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

        public async Task<IActionResult> Index()
        {
            var especimenes = await _apiData.ListarAsync();
            return View(especimenes);
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
    }
}