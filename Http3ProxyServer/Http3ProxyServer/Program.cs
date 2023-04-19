using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);


builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Listen(IPAddress.Any, 7192, listenOptions =>
    {
        // Use HTTP/3
        listenOptions.Protocols = HttpProtocols.Http2; // TODO: change to Http3 if on a platform that supports it
        listenOptions.UseHttps();
    });
}
);

var app = builder.Build();

app.MapGet("/", async httpContext =>
{
    await httpContext.Response.WriteAsync("HTT2 Test!");
});

app.Run();
