using System.Net;
using Http3ProxyServer;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Listen(IPAddress.Any, 7192, listenOptions =>
    {
        // Limit connections to HTTP/3
        listenOptions.Protocols = HttpProtocols.Http3; 
        listenOptions.UseHttps();
    });
}
);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.UseMiddleware<ReverseProxyMiddleware>();

app.MapGet("/", async httpContext =>
{
    await httpContext.Response.WriteAsync("HTTP3 Test!");
});

app.Run();  // Throws an error!
            // "Platform doesn't support QUIC or HTTP/3."
            // Windows 10 does not have MSQuic support
