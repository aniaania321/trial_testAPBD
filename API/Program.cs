using Logic;
using Model;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddTransient<ICurrencyService, CurrencyService>(_ => new CurrencyService(connectionString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.MapPost("/api/currency/{currencyName}/{rate}/{countries}",
    (ICurrencyService currencyService, string currencyName, float rate, string countries) =>
    {
        var countryList = countries.Split(',').ToList();
        currencyService.CreateOrUpdate(currencyName, rate, countryList);
        return Results.Ok("Currency created or updated!");
    });

app.MapGet("/api/currency/{type}",
    (ICurrencyService currencyService, string type,string? countryName, string? currencyName) =>
    {
        if (type=="ByCountry")
        {
            var result=currencyService.SearchByCountry(countryName);
            return Results.Ok(result);
        }
        else if (type=="ByCurrency")
        {
            var result=currencyService.SearchByCurrency(currencyName);
            return Results.Ok(result);
        }
        return Results.BadRequest();
    });
app.MapDelete("/api/currency/{type}",
    (ICurrencyService currencyService, string type,string? countryName, string? currencyName) =>
    {
        if (type=="Country")
        {
            currencyService.DeleteCountry(countryName);
            return Results.Ok("Deleted country");
        }
        else if (type=="Currency")
        {
            currencyService.DeleteCurrency(currencyName);
            return Results.Ok("Deleted currency");
        }
        return Results.BadRequest();
    });

app.Run();
