using System.Xml.Schema;
using Microsoft.Data.SqlClient;
using Model;

namespace Logic;

public class CurrencyService:ICurrencyService
{
    private string _connectionString;
    public CurrencyService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void CreateOrUpdate(string CurrencyName, float CurrencyRate, List<string> CountryList)
{
    List<Country> Countries = new List<Country>();
    for (int i = 0; i < CountryList.Count; i++)
    {
        string countryString = CountryList[i];
        Country country = new Country(countryString);
        Countries.Add(country);
    }

    Currency newCurrency = new Currency(CurrencyName, CurrencyRate);
    int currency_Id = 0;

    string queryString = "INSERT INTO Currency(Name, Rate) VALUES(@CurrencyName, @CurrencyRate); SELECT CAST(SCOPE_IDENTITY() as int);";
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        SqlCommand command = new SqlCommand(queryString, connection);
        connection.Open();
        command.Parameters.AddWithValue("@CurrencyName", newCurrency.Name);
        command.Parameters.AddWithValue("@CurrencyRate", CurrencyRate);
        currency_Id = (int)command.ExecuteScalar();
    }

    string queryString4 = "SELECT Id FROM Country WHERE Name=@CountryName";
    string queryString5 = "INSERT INTO Currency_Country(Currency_Id,Country_id) VALUES(@CurrencyId,@CountryId)";
    using (SqlConnection connection = new SqlConnection(_connectionString))
    {
        connection.Open();  // Make sure connection is open

        SqlCommand command2 = new SqlCommand(queryString4, connection);
        List<int> ids = new List<int>();
        foreach (string countryName in CountryList)
        {
            int country_id;
            command2.Parameters.Clear();
            command2.Parameters.AddWithValue("@CountryName", countryName);
            SqlDataReader reader2 = command2.ExecuteReader();
            try
            {
                if (reader2.HasRows)
                {
                    while (reader2.Read())
                    {
                        country_id = reader2.GetInt32(0);
                        ids.Add(country_id);
                    }
                }
            }
            finally
            {
                reader2.Close();
            }
        }

        SqlCommand command3 = new SqlCommand(queryString5, connection);
        for (int i = 0; i < CountryList.Count; i++)
        {
            command3.Parameters.Clear();
            command3.Parameters.AddWithValue("@CurrencyId", currency_Id);
            command3.Parameters.AddWithValue("@CountryId", ids[i]);
            command3.ExecuteNonQuery();
        }
    }
}


    public string SearchByCountry(string countryName)
    {
        int country_Id = getCountryId(countryName); // zakładam że to działa i zwraca ID

        string query = @"
        SELECT c.Name, c.Rate 
        FROM Currency c
        JOIN Currency_Country cc ON c.Id = cc.Currency_Id
        WHERE cc.Country_Id = @Country_Id";

        List<Currency> currencies = new List<Currency>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@Country_Id", country_Id);
            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string currencyName = reader.GetString(0);
                    float currencyRate = reader.GetFloat(1);

                    currencies.Add(new Currency(currencyName, currencyRate));
                }
            }
        }

        string result = "";

        foreach (var currency in currencies)
        {
            result = result + currency.Name;
            
        }

        return countryName + "; "+result;
    }


    public int getCountryId(string CountryName)
    {
        string query = "SELECT Id FROM Country WHERE Name=@CountryName";
        int country_Id = -1;
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            command.Parameters.AddWithValue("@CountryName", CountryName);
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                country_Id = reader.GetInt32(0);
            }
        }
        return country_Id;
        
    }
    public int getCurrencyId(string CurrencyName)
    {
        string query = "SELECT Id FROM Currency WHERE Currency Name=@CurrencyName";
        int currency_Id = -1;
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                currency_Id = reader.GetInt32(0);
            }
        }
        return currency_Id;
        
    }

    public string SearchByCurrency(string CurrencyName)
    {
        throw new NotImplementedException();
    }
}