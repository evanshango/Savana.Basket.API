using System.Text.Json.Serialization;
using log4net;
using MassTransit;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Savana.Basket.API.Consumers;
using Savana.Basket.API.Interfaces;
using Savana.Basket.API.Services;
using StackExchange.Redis;
using Treasures.Common.Extensions;
using Treasures.Common.Interfaces;
using Treasures.Common.Middlewares;
using Treasures.Common.Services;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<IHistoryService, HistoryService>();

// Add MassTransit
builder.Services.AddMassTransit(config => {
    config.AddConsumer<BasketConsumer>();
    config.UsingRabbitMq((ctx, cfg) => {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], h => {
                h.Username(builder.Configuration["RabbitMQ:Username"]);
                h.Password(builder.Configuration["RabbitMQ:Password"]);
            }
        );
        cfg.ReceiveEndpoint("basket-events", c =>
            c.ConfigureConsumer<BasketConsumer>(ctx)
        );
    });
});

builder.Services.AddLogging(c => c.ClearProviders());
builder.Logging.AddLog4Net();

GlobalContext.Properties["pid"] = Environment.ProcessId;
GlobalContext.Properties["appName"] = builder.Configuration["Properties:Name"];

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
    { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto }
);
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment()) app.UseSavanaSwaggerDoc("Savana Basket API Service v1");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var address = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")!.Split(";").First();
app.Logger.LogInformation("Savana Basket.API started on {Addr}", address);

app.Run();