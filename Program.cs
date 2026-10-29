var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => DateTime.Now.ToString());

app.Run();
