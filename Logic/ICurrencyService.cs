namespace Logic;

public interface ICurrencyService
{
    public void CreateOrUpdate(string CurrencyName, float CurrencyRate, List<string> CountryList);
    public string SearchByCountry(string CountryName);
    public string SearchByCurrency(string CurrencyName);
    public void DeleteCurrency(string CurrencyName);
    public void DeleteCountry(string CountryName);
}