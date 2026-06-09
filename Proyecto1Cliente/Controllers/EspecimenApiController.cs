using Microsoft.AspNetCore.Mvc;
using Proyecto1Cliente.Models.Data;
using Proyecto1Cliente.Models.Domain;

namespace Proyecto1Cliente.Controllers
{
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
    }
}