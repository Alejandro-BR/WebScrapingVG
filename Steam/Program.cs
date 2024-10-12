using Microsoft.Playwright;

namespace Steam;

internal class Program
{
    public static async Task Main()
    {
        string[] nombresJuegos = {
            "The Witcher 3: Wild Hunt",
            "Baldur's Gate 3",
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

        Console.WriteLine("Lista de juegos:\n");

        List<Juego> juegos = await getInfoJuegos(nombresJuegos);

        Console.WriteLine(juegos.Count);

    }

    /**
    * - Intenta seleccionar el precio del producto desde el elemento HTML.
    * - Intenta seleccionar el nombre del producto desde el elemento HTML.
    * Si tiene éxito, retorna un objeto Juego con el nombre y el precio.
    * Si falla, captura y lanza un error.
    * 
    * @param {IElement} element - El elemento HTML que representa el producto
    * @return {Promise<Juego>} -Un Objeto con el nombre del juego y precio del producto
    * **/
    private static async Task<Juego> GetProductAsync(IElementHandle element)
    {
        // DOM
        const string PRECIO_DOM = ".discount_final_price";
        const string TITULO_DOM = ".title";

        // PRECIO
        // Referencia le span con texto
        IElementHandle precioElement = await element.QuerySelectorAsync(PRECIO_DOM); 
        string precioRaw = await precioElement.InnerTextAsync(); // Coge el texto del span
        // Quitar el €
        precioRaw = precioRaw.Replace("€", "",
        StringComparison.OrdinalIgnoreCase);
        // Quitar los espacios al principio y al final de la cadena
        precioRaw = precioRaw.Trim();
        // Pasar a decimal
        decimal precio = decimal.Parse(precioRaw);

        // TITULO
        // Referencia le span con texto
        IElementHandle tituloElement = await element.QuerySelectorAsync(TITULO_DOM);
        string titulo = await tituloElement.InnerTextAsync();  // Coge el texto del span

        // Devolver el producto
        return new Juego(titulo, precio);
    }

    /**
    * - Intenta obtener información de juegos desde el sitio web de Steam.
    * - Itera sobre los nombres de juegos proporcionados y busca cada uno en el sitio.
    * - Si encuentra un juego, recolecta su nombre y precio, y crea un objeto Juego.
    * - Si falla en alguna búsqueda, captura y muestra el error en consola.
    * 
    * @param {string[]} nombresJuegos - Un array de cadenas que contiene los nombres de los juegos a buscar.
    * @return {Promise<List<Juego>>} juegosDatos - Devuelve todos los juegos en una lista.
    */
    private static async Task<List<Juego>> getInfoJuegos(string[] nombresJuegos)
    {
        // Almacena todos los juegos
        List<Juego> juegosDatos = new List<Juego>();

        // DOM
        const string BOTON_COOKIS = ".btn_blue_steamui.btn_medium";
        const string BARRA_BUSQUEDA = "#store_nav_search_term";
        const string RESULTADO = "#search_resultsRows a";

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
        IElementHandle? acceptButton = await page.QuerySelectorAsync(BOTON_COOKIS);
        if (acceptButton != null) await acceptButton.ClickAsync();

        foreach (var nombreJ in nombresJuegos)
        {
            // Escribir en la barra de busqueda
            IElementHandle searchInput = await page.QuerySelectorAsync(BARRA_BUSQUEDA);
            await searchInput.FillAsync(nombreJ);

            // Simular la tecla Enter para buscar
            await searchInput.PressAsync("Enter");

            await Task.Delay(2000);

            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            // Recorremos la lista de productos y recolectamos los datos
            List<Juego> juegos = new List<Juego>();
            IReadOnlyList<IElementHandle> juegosElements = await page.QuerySelectorAllAsync(RESULTADO);

            IElementHandle first = juegosElements[0];
            Juego juego = await GetProductAsync(first);

            juegosDatos.Add(juego);

            Console.WriteLine(juego);
        }

        return juegosDatos;

    }
}
