using CSHarpC1.Models;
using CSHarpC1.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Database
builder.Services.AddScoped<SchoolDbContext>();

// builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add this near other service registrations
builder.Services.AddScoped<CSHarpC1.Controllers.TeacherAPIController>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Add conventional routing before app.MapControllers()
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=TeacherPage}/{action=List}/{id?}");

// Keep the app.MapControllers() line too - it maps attribute routes
app.MapControllers();


app.Run();