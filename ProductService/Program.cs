using System.Net;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
            
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<AdminSafeListMiddleware>("145.93.77.6;145.93.76.86;51.116.145.38;10.0.0.7;192.168.1.5;::1;145.93.116.244");
//Gateway ip:       51.116.145.38
//Brand service ip:     
app.Run();


public class AdminSafeListMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AdminSafeListMiddleware> _logger;
    private readonly byte[][] _safelist;

    public AdminSafeListMiddleware(
        RequestDelegate next,
        ILogger<AdminSafeListMiddleware> logger,
        string safelist)
    {
        var ips = safelist.Split(';');
        _safelist = new byte[ips.Length][];
        for (var i = 0; i < ips.Length; i++)
        {
            _safelist[i] = IPAddress.Parse(ips[i]).GetAddressBytes();
        }

        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Method == HttpMethod.Get.Method)
        {
            var remoteIp = context.Connection.RemoteIpAddress;
            _logger.LogDebug("Request from Remote IP address: {RemoteIp}", remoteIp);

            var bytes = remoteIp.GetAddressBytes();
            var badIp = true;
            foreach (var address in _safelist)
            {
                if (address.SequenceEqual(bytes))
                {
                    badIp = false;
                    break;
                }
            }

            if (badIp)
            {
                _logger.LogWarning(
                    "Forbidden Request from Remote IP address: {RemoteIp}", remoteIp);
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                
                return;
            }
        }

        await _next.Invoke(context);
    }
}
