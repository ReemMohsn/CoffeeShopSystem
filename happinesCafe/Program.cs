using happinesCafe.Services;
using happinesCafe.DATA;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    EnvironmentName = Environments.Development, // √Ê Environments.Production
    Args = args
});

//var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<CaffeeSystemContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("Myconnection")
    ));

builder.Services.AddDistributedMemoryCache();
// ≈÷«›… Œœ„«  «·Ã·”« 

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // „œ… «‰ Â«¡ «·Ã·”…
    options.Cookie.HttpOnly = true; //  √„Ì‰ «·ﬂÊﬂÌ
    options.Cookie.IsEssential = true; // Ã⁄· «·ﬂÊﬂÌ √”«”Ì«
});


builder.Services.AddTransient<IEmailService, EmailService>();

var app = builder.Build();


//  ﬂÊÌ‰ ŒÿÊÿ «·√‰«»Ì» (Pipeline)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();





app.UseStaticFiles();

app.UseRouting();
// «” Œœ«„ «·Ã·”« 
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
