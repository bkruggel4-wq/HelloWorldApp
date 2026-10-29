var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Today is the ${DateTime.Now.ToString()});

app.Run();
