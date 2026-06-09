using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Proyecto1Cliente.Models.Data;
using System.Security.Claims;

namespace Proyecto1Cliente.Controllers
{
    public class SessionController : Controller
    {
        private readonly UsuarioApiData _apiData;

        public SessionController(UsuarioApiData apiData)
        {
            _apiData = apiData;
        }

        // GET: Mostrar pantalla de login
        public IActionResult Login()
        {
            // Si ya está logueado, mandarlo al inicio
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "EspecimenApi");

            return View();
        }

        // POST: Procesar credenciales
        [HttpPost]
        public async Task<IActionResult> Login(string correo, string contrasena)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
            {
                ViewBag.Error = "Completá todos los campos.";
                return View();
            }

            var usuario = await _apiData.LoginAsync(correo, contrasena);

            if (usuario == null)
            {
                ViewBag.Error = "Correo no registrado o contraseña incorrecta.";
                return View();
            }

            // Crear la identidad del usuario ("La Galleta" de seguridad)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Role, usuario.NombreRol)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Iniciar sesión en .NET
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "EspecimenApi");
        }

        // GET: Cerrar sesión
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}