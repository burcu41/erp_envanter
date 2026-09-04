using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors("AllowAll");

// MSSQL BAGLANTI TESTI
var connectionString = app.Configuration.GetConnectionString("DefaultConnection");

try
{
    using (var connection = new SqlConnection(connectionString))
    {
        connection.Open();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n==========================================");
        Console.WriteLine(">>> MSSQL Connected - EnvanterDB Baglantisi Basarili! <<<");
        Console.WriteLine("==========================================\n");
        Console.ResetColor();
    }
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("\n==========================================");
    Console.WriteLine($">>> MSSQL Baglanti Hatasi: {ex.Message} <<<");
    Console.WriteLine("==========================================\n");
    Console.ResetColor();
}

app.MapGet("/", () => "ERP Envanter API Calisiyor!");

app.Run();