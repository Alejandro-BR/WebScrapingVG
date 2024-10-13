namespace WebScraping;

internal class Program
{
    static async Task Main(string[] args) // Cambiado a async
    {
        string[] nombresJuegos = {
            "The Witcher 3: Wild Hunt",
            "The Elder Scrolls V: Skyrim Special Edition",
            "Resident Evil 2",
            "God of War",
            "Frostpunk 2",
            "Terraria",
            "Spore",
            "Cyberpunk 2077: Ultimate Edition",
            "Disney Epic Mickey: Rebrushed",
            "GRIS"
        };

        // Instalar los navegadores
        Microsoft.Playwright.Program.Main(new[] { "install" });

        List<Juego> juegosSteam = await Steam.GetJuegos(nombresJuegos);
        List<Juego> juegosInstantGaming = await InstantGaming.GetJuegos(nombresJuegos);
        List<Juego> juegosGog = await Gog.GetJuegos(nombresJuegos);

        Console.WriteLine("\n---------------------------\n");

        for (int i = 0; i < nombresJuegos.Length; i++)
        {
            Console.WriteLine("\n" + nombresJuegos[i]);
            Console.WriteLine("Steam: \n" + juegosSteam[i]);
            Console.WriteLine("Instant Gaming: \n" + juegosInstantGaming[i]);
            Console.WriteLine("GOG: \n" + juegosGog[i]);
        }
    }
}
