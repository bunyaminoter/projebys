using Microsoft.EntityFrameworkCore;
using projebys.Data;

var builder = WebApplication.CreateBuilder(args);

// Veritabaný baðlantýsý
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Session için gerekli ayarlar
builder.Services.AddDistributedMemoryCache(); // Bellek tabanlý cache
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 30 dakika boyunca geçerli olacak
    options.Cookie.HttpOnly = true; // Sadece HTTP üzerinden eriþilebilir
    options.Cookie.IsEssential = true; // Çerez zorunlu hale gelir
});

// Razor Pages ve Controller'lar için servis ekleme
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// API için Swagger desteði
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Swagger entegrasyonu

// IHttpClientFactory servisini ekliyoruz (API istekleri için)
builder.Services.AddHttpClient(); // Bu satýr IHttpClientFactory'yi ekler

// CORS yapýlandýrmasý - API'yi farklý domainlerden eriþilebilir hale getirmek için
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()  // Herhangi bir kaynaða izin verir
              .AllowAnyMethod()  // Herhangi bir HTTP metoduna izin verir
              .AllowAnyHeader(); // Herhangi bir baþlýða izin verir
    });
});

var app = builder.Build();

// Middleware ekleme
app.UseRouting();

// CORS middleware ekleyerek, tüm API'lere dýþarýdan eriþimi saðlýyoruz
app.UseCors("AllowAllOrigins");

// Session middleware'ini ekliyoruz
app.UseSession();

// Swagger kullanýmý (geliþtirme ortamý için)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Statik dosyalar için

app.UseAuthorization(); // Kullanýcý yetkilendirme

// Razor Pages ve API controller'larý için route'lar
app.MapRazorPages();
app.MapControllers();

app.Run();
