using Asp.Versioning;
using Books.API.Middlewares;
using Books.API.Swagger;
using Books.Application;
using Books.Application.Database;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Adding versioning
builder.Services.AddApiVersioning(x =>
{
    x.DefaultApiVersion = ApiVersion.Default; // 1.0 it's the default
    // If we do not specify that assume default is available we
    // end up returning 400 Bad request with a message that basically tells that
    // the Api version is required
    x.AssumeDefaultVersionWhenUnspecified = true;
    // Reporting api versions will return the headers
    // "api-supported-versions" and "api-deprecated-versions"
    x.ReportApiVersions = true;
    // The MediaTypeApiVersion is basically to pass the version from
    // within the Accept header
    // for example: Accept: application/json;api-version=1.0
    // x.ApiVersionReader = new MediaTypeApiVersionReader("api-version");
    // Specifying a custom attribute is as easy as change the reader
    // x.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
})
    .AddMvc()
    .AddApiExplorer(o =>
    {
        o.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddApplication();
builder.Services.AddDatabase(config["Database:ConnectionString"]!);

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigSwaggerOptions>();
builder.Services.AddSwaggerGen(x => x.OperationFilter<SwaggerDefaultValues>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(b =>
    {
        foreach (var description in app.DescribeApiVersions())
        {
            b.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                description.GroupName);
        }
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ValidationMappingMiddleware>();

app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();

app.Run();
