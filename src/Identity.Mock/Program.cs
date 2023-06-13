using Identity.Mock.Token;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Register services

var app = builder.Build();

app.MapGet("/", () => "This is an Identity Mock for show up the flow of authentication and authorization");
app.MapCreateToken(config);


app.Run();
