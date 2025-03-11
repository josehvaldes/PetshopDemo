using Asp.Versioning;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.ML;
using Microsoft.OpenApi.Models;
using Okta.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version")
    );
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

//builder.Services.AddPredictionEnginePool<object, object>()
//    .FromUri(modelName: "", uri: "");


var oktasetting = builder.Configuration.GetSection("OktaSettings");

//Add Okta authentication
builder.Services.AddAuthentication(
    options =>
    {
        options.DefaultAuthenticateScheme = OktaDefaults.ApiAuthenticationScheme;
        options.DefaultChallengeScheme = OktaDefaults.ApiAuthenticationScheme;
        options.DefaultSignInScheme = OktaDefaults.ApiAuthenticationScheme;
    }
).AddOktaWebApi(new OktaWebApiOptions()
{
    OktaDomain = oktasetting.GetValue<string>("urlDomain")
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ML API - V1", Version = "v1.0" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "ML API - V2", Version = "v2.0" });
});

//Add support to logging with SERILOG
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
    //configuration.WriteTo.ApplicationInsights(
    //        services.GetRequiredService<TelemetryConfiguration>(), TelemetryConverter.Traces
    //        );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
