using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Cliente HTTP para Especímenes
builder.Services.AddHttpClient<Proyecto1Cliente.Models.Data.EspecimenApiData>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ProyectoApi:BaseUrl"] ?? "http://127.0.0.1/Proyecto1Admin/SW.php");
});

// ¡NUEVO! Cliente HTTP para Usuarios (Necesario para el Login)
builder.Services.AddHttpClient<Proyecto1Cliente.Models.Data.UsuarioApiData>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ProyectoApi:BaseUrl"] ?? "http://127.0.0.1/Proyecto1Admin/SW.php");
});

// ¡NUEVO! Configuración de Autenticación por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Session/Login"; // Redirige aquí si intentan entrar sin sesión
        options.AccessDeniedPath = "/Session/Login";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// ¡NUEVO! IMPORTANTE: UseAuthentication debe ir estrictamente ANTES de UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Session}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();