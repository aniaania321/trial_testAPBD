namespace Model;

public class Currency
{
    public string Name { get; set; }
    public float Rate { get; set; }

    public Currency(string Name, float Rate)
    {
        this.Name = Name;
        this.Rate = Rate;
    }

}