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
        if (string.IsNullOrWhiteSpace(currencyName))
            return Results.BadRequest("Currency name cannot be empty.");

        if (rate <= 0)
            return Results.BadRequest("Rate must be greater than zero.");

        if (string.IsNullOrWhiteSpace(countries))
            return Results.BadRequest("At least one country must be provided.");
        
        var countryList = countries.Split(',').ToList();
        currencyService.CreateOrUpdate(currencyName, rate, countryList);
        return Results.Ok("Currency created or updated!");
    });

app.MapGet("/api/search/{type}",//tak gdy opcjonalny argument
    (ICurrencyService currencyService, string type, string? countryName, string? currencyName) =>
    {
        try
        {
            if (type == "ByCountry")
            {
                var result = currencyService.SearchByCountry(countryName!);
                return Results.Ok(result);
            }
            else if (type == "ByCurrency")
            {
                var result = currencyService.SearchByCurrency(currencyName!);
                return Results.Ok(result);
            }

            return Results.BadRequest("Invalid type specified.");
        }
        catch (NotFoundException ex)
        {
            return Results.NotFound(ex.Message); // a tutaj sprawdzam czy ten błąd nie był wyrzucony
        }
    });

app.MapDelete("/api/currency/{type}",
    (ICurrencyService currencyService, string type,string? countryName, string? currencyName) =>
    {
        if (type=="Country")
        {
            if (string.IsNullOrWhiteSpace(countryName))
                return Results.BadRequest("countryName is required for deleting a country.");
            
            currencyService.DeleteCountry(countryName);
            return Results.Ok("Deleted country");
        }
        else if (type=="Currency")
        {
            if (string.IsNullOrWhiteSpace(currencyName))
                return Results.BadRequest("currencyName is required for deleting a currency.");
            
            currencyService.DeleteCurrency(currencyName);
            return Results.Ok("Deleted currency");
        }
        return Results.BadRequest();
    });

app.Run();
