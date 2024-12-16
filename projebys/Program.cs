using Microsoft.EntityFrameworkCore;
using projebys.Data;

var builder = WebApplication.CreateBuilder(args);

// Veritaban� ba�lant�s�
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Session i�in gerekli ayarlar
builder.Services.AddDistributedMemoryCache(); // Bellek tabanl� cache
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 30 dakika boyunca ge�erli olacak
    options.Cookie.HttpOnly = true; // Sadece HTTP �zerinden eri�ilebilir
    options.Cookie.IsEssential = true; // �erez zorunlu hale gelir
});

// Razor Pages ve Controller'lar i�in servis ekleme
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// API i�in Swagger deste�i
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Swagger entegrasyonu

// IHttpClientFactory servisini ekliyoruz (API istekleri i�in)
builder.Services.AddHttpClient(); // Bu sat�r IHttpClientFactory'yi ekler

// CORS yap�land�rmas� - API'yi farkl� domainlerden eri�ilebilir hale getirmek i�in
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()  // Herhangi bir kayna�a izin verir
              .AllowAnyMethod()  // Herhangi bir HTTP metoduna izin verir
              .AllowAnyHeader(); // Herhangi bir ba�l��a izin verir
    });
});

var app = builder.Build();

// Middleware ekleme
app.UseRouting();

// CORS middleware ekleyerek, t�m API'lere d��ar�dan eri�imi sa�l�yoruz
app.UseCors("AllowAllOrigins");

// Session middleware'ini ekliyoruz
app.UseSession();

// Swagger kullan�m� (geli�tirme ortam� i�in)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Statik dosyalar i�in

app.UseAuthorization(); // Kullan�c� yetkilendirme

// Razor Pages ve API controller'lar� i�in route'lar
app.MapRazorPages();
app.MapControllers();

app.Run();
