using System.Text;

namespace Steam;

internal class Juego
{
    public string Name { get; set; }
    public string Url { get; set; }
    public decimal Price { get; set; }

    public Juego(string name, string url, decimal price)
    {
        Name = name;
        Price = price;
        Url = url;
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"Name: {Name}");
        stringBuilder.AppendLine($"Url: {Url}");
        stringBuilder.AppendLine($"Price: {Price}");
        return stringBuilder.ToString();
    }
}
