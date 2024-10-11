using Microsoft.Playwright;

namespace Steam;

internal class Program
{
    public static async Task Main()
    {
        string[] nombresJuegos = {
            "The Witcher 3: Wild Hunt",
            "DRAGON BALL: Sparking! ZERO",
            "Black Myth: Wukong",
            "God of War",
            "Red Dead Redemption 2",
            "Terraria",
            "Detroit: Become Human",
            "Cyberpunk 2077: Ultimate Edition",
            "Assassin's Creed® Odyssey",
            "Stray"
        };

        // Instalar los navegadores
        Microsoft.Playwright.Program.Main(["install"]);

        foreach (var juego in nombresJuegos)
        {
            await getInfoJuegos(juego);
        }


        // Espera infinita
        //await Task.Delay(-1);
    }

    private static async Task<Juego> GetProductAsync(IElementHandle element, string nombre)
    {
        // PRECIO
        IElementHandle priceElement = await
        element.QuerySelectorAsync(".discount_final_price"); // Referencia le span con texto
        string priceRaw = await priceElement.InnerTextAsync(); // Coge el texto del span
        // Quitar el EUR
        priceRaw = priceRaw.Replace("€", "",
        StringComparison.OrdinalIgnoreCase);
        // Quitar los espacios al principio y al final de la cadena
        priceRaw = priceRaw.Trim();
        // Pasar a decimal
        decimal price = decimal.Parse(priceRaw);

        // Devolver el producto
        return new Juego(nombre, price);
    }

    private static async Task getInfoJuegos(string nombreJ)
    {
        // Crear Playwright
        using IPlaywright playwright = await Playwright.CreateAsync();

        // Opciones para el navegador
        BrowserTypeLaunchOptions options = new BrowserTypeLaunchOptions()
        {
            Headless = false // Se indica falso para poder ver el navegador
        };

        // Crear el navegador
        await using IBrowser browser = await playwright.Chromium.LaunchAsync(options);

        // Crear un nuevo contexto y página
        await using IBrowserContext context = await browser.NewContextAsync();
        IPage page = await context.NewPageAsync();

        // Ir a la pagina de Steam
        await page.GotoAsync("https://store.steampowered.com/?l=spanish");

        // Delay
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // Aceptar cookies si aparece el boton
        IElementHandle? acceptButton = await page.QuerySelectorAsync(".btn_blue_steamui.btn_medium");
        if (acceptButton != null) await acceptButton.ClickAsync();

        // Escribir en la barra de busqueda
        IElementHandle searchInput = await page.QuerySelectorAsync("#store_nav_search_term");
        await searchInput.FillAsync(nombreJ);

        // Simular la tecla Enter para buscar
        await searchInput.PressAsync("Enter");

        await Task.Delay(2000);

        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // Recorremos la lista de productos y recolectamos los datos
        List<Juego> juegos = new List<Juego>();
        IReadOnlyList<IElementHandle> juegosElements = await page.QuerySelectorAllAsync("#search_resultsRows a");

        IElementHandle first = juegosElements[0];
        Juego juego = await GetProductAsync(first, nombreJ);

        Console.WriteLine(juego);
    }
}
