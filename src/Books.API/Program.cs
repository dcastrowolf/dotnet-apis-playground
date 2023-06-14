using Asp.Versioning;
using Books.API.Auth;
using Books.API.Middlewares;
using Books.API.Swagger;
using Books.Application;
using Books.Application.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.

// Add authentication and configure the defaults with the strategy of
// a Bearer Token.
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        ValidateIssuer = true,
        ValidateAudience = true
    };
});

// Add Authorization
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy(AuthConstants.AdminUserPolicyName,
        p => p.RequireClaim(AuthConstants.AdminUserClaimName, "true"));

    opt.AddPolicy(AuthConstants.TrustedMemberPolicyName,
        p => p.RequireAssertion(c =>
            c.User.HasClaim(m => m is { Type: AuthConstants.AdminUserClaimName, Value: "true" }) ||
            c.User.HasClaim(m => m is { Type: AuthConstants.TrustedMemberClaimName, Value: "true" })));

});

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

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ValidationMappingMiddleware>();

app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();

app.Run();
