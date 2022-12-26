using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Treasures.Common.Extensions;
using Treasures.Common.Interfaces;
using Treasures.Common.Middlewares;
using Treasures.Common.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(opt => {
    opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
}).ConfigureApiBehaviorOptions(x => { x.SuppressMapClientErrors = true; });
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerAuthenticated("Savana Basket API Service", "v1");
builder.Services.AddErrorResponse<ApiBehaviorOptions>();

builder.Services.AddRouting(opt => {
    opt.LowercaseUrls = true;
    opt.LowercaseQueryStrings = true;
});

builder.Services.AddSingleton<IConnectionMultiplexer>(_ => {
    var config = ConfigurationOptions.Parse(
        builder.Configuration.GetConnectionString("Redis"), true
    );
    return ConnectionMultiplexer.Connect(config);
});

builder.Services.AddScoped(typeof(ICacheService<>), typeof(CacheService<>));

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
    { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto }
);
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment()) app.UseSavanaSwaggerDoc("Savana Basket API Service v1");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();