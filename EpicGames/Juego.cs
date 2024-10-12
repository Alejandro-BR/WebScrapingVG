using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GOG;
internal class Juego
{

    public string Name { get; init; } //El init significa que puedes añadirle un valor al crearlo unicamente
    public decimal Price { get; init; }
    public Juego(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"Name: {Name}");
        stringBuilder.AppendLine($"Price: {Price}");

        return stringBuilder.ToString();
    }
}
