namespace Model;

public class Currency_Country
{
    public int Country_Id { get; set; }
    public int Currency_Id { get; set; }

    public Currency_Country(int Currency_Id, int Country_Id)
    {
        this.Country_Id = Country_Id;
        this.Currency_Id = Currency_Id;
    }
    
}