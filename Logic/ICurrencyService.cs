namespace Logic;

public interface ICurrencyService
{
    public void CreateOrUpdate(string CurrencyName, float CurrencyRate, List<string> CountryList);
    public string SearchByCountry(string CountryName);
    public string SearchByCurrency(string CurrencyName);
}