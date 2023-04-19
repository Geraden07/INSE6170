using System.Net;

HttpClient client = new HttpClient();
client.DefaultRequestVersion = HttpVersion.Version30;
client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact;

var response = await client.GetAsync("https://localhost:7192/");
var body = await response.Content.ReadAsStringAsync();

Console.WriteLine($"status: {response.StatusCode}, version: {response.Version}, body: {body}");
