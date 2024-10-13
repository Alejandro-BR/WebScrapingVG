using System.Text;


namespace WebScraping;

internal class Juego
{
    public string Name { get; set; }
    public decimal Price { get; set; }

    public Juego(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    public Juego(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"Name: {Name}");
        stringBuilder.AppendLine($"Price: {Price}");
        return stringBuilder.ToString();
    }
}

