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

app.MapGet("/api/currency/{countryName}",
    (ICurrencyService currencyService, string countryName) =>
    {
        var result=currencyService.SearchByCountry(countryName);
        return Results.Ok(result);
    });

app.Run();
