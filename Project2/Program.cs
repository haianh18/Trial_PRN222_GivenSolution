using Microsoft.EntityFrameworkCore;
using Project2;
using Project2.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PePrn222TrialContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

builder.Services.AddScoped<PePrn222TrialContext>();

var app = builder.Build();
app.UseStaticFiles(); // Phải có dòng này

app.MapRazorPages();

app.MapGet("/", () => "Hello World!");

app.MapHub<ChatHub>("/chatHub");

app.Run();
