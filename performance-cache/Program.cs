using Repository;
using Service;
using System.Net; // Re-adicionando para o tratamento de exceção
using Microsoft.Extensions.Logging; // Re-adicionando para o tratamento de exceção
using Domain; // Adicionando para usar as exceções personalizadas
using Persistence;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Configuração do Entity Framework Core com provedor In-Memory (equivalente ao H2)
builder.Services.AddDbContext<EstoqueContext>(options =>
    options.UseInMemoryDatabase("EstoqueDB")
);
builder.Services.AddSwaggerGen();

// Configuração dos Repositórios (Persistence)
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IMovimentacaoRepository, MovimentacaoRepository>();

// Configuração dos Serviços (Service)
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IMovimentacaoService, MovimentacaoService>();
builder.Services.AddScoped<IRelatorioService, RelatorioService>();

// builder.Services.AddScoped<IVehicleRepository, VehicleRepository>(provider =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                           ?? "Server=localhost;Database=fiap;User=root;Password=123;Port=3306;";
    
    return new VehicleRepository(connectionString);
});
// Fim da injeção de dependência original (comentada para evitar conflitos)

// Registrar o CacheService
builder.Services.AddSingleton<ICacheService, CacheService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware global de tratamento de exceções
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        if (exception != null)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(exception, "Erro capturado pelo middleware global");

            // Mapeamento de Exceções Personalizadas para Status HTTP
            context.Response.StatusCode = exception switch
            {
                Domain.ProdutoNaoEncontradoException => (int)System.Net.HttpStatusCode.NotFound, // 404
                Domain.ValidacaoException => (int)System.Net.HttpStatusCode.BadRequest, // 400
                Domain.EstoqueInsuficienteException => (int)System.Net.HttpStatusCode.BadRequest, // 400
                _ => (int)System.Net.HttpStatusCode.InternalServerError // 500
            };

            var errorResponse = new
            {
                status = context.Response.StatusCode,
                message = exception.Message,
                timestamp = DateTime.UtcNow,
                requestId = context.TraceIdentifier
            };

            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(errorResponse));
        }
    });
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
