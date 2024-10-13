using Microsoft.Playwright;
using System.Globalization;

namespace WebScraping;

internal class InstantGaming
{
    /**
    * Metodo main
    *
    * @param {string[]} nombresJuegos - Array de nombres que hay que buscar
    * @return {Promise<List<Juego>>} - Lista de los juegos
    */
    public static async Task<List<Juego>> GetJuegos(string[] nombresJuegos)
    {
        Console.WriteLine("\nInstant Gaming --> Lista de juegos:\n");

        List<Juego> juegos = await getInfoJuegos(nombresJuegos);

        Console.WriteLine(juegos.Count);

        return juegos;
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
        // PRECIO
        IElementHandle priceElement = await
        element.QuerySelectorAsync(".information .price"); // Referencia le span con texto
        string priceRaw = await priceElement.InnerTextAsync(); // Coge el precio del span
        // NOMBRE
        IElementHandle nameElement = await
        element.QuerySelectorAsync(".information .text"); // Referencia le span con texto
        string textName = await nameElement.InnerTextAsync(); // Coge el texto del span
        // Quitar el EUR
        priceRaw = priceRaw.Replace("€", "", StringComparison.OrdinalIgnoreCase);

        // Cambiar el punto por una coma
        //priceRaw = priceRaw.Replace(".", ",", StringComparison.OrdinalIgnoreCase);

        // Quitar los espacios al principio y al final de la cadena
        priceRaw = priceRaw.Trim();
        // Pasar a decimal
        decimal price = decimal.Parse(priceRaw);

        // Devolver el producto
        return new Juego(textName, price);
    }


    /**
     * - Intenta obtener información de juegos desde el sitio web de Instant Gaming.
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

        //Sirve para obtener el precio del modo en al es
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        // Inicializa Playwright para manejar la automatización del navegador
        using IPlaywright playwright = await Playwright.CreateAsync();

        BrowserTypeLaunchOptions options = new BrowserTypeLaunchOptions()
        {
            Headless = false // Se indica falso para poder ver el navegador
        };
        //Lnaza navegador
        await using IBrowser browser = await playwright.Chromium.LaunchAsync(options); //limpia navegador
        await using IBrowserContext context = await browser.NewContextAsync();
        IPage page = await context.NewPageAsync(); //Abre pagina

        // Ir a la página principal de Instant Gaming
        await page.GotoAsync("https://www.instant-gaming.com/");


        // Aceptar condiciones de Instant Gaming (cookies)
        IElementHandle acceptButton = await page.QuerySelectorAsync("//button[text()='Aceptar todo']");
        if (acceptButton != null) await acceptButton.ClickAsync(); //Hace click en el boton de aceptar cookies
        await Task.Delay(2000);//Espera de 2 milisegundos esperando cookies


        // Le damos al botón de buscar del encabezado
        IElementHandle searchButton = await page.QuerySelectorAsync(".icon-search-input");
        await searchButton.ClickAsync();

        foreach (var nombre in nombresJuegos)
        {
            // Escribimos en la barra de búsqueda lo que queremos buscar
            IElementHandle searchInput = await page.QuerySelectorAsync("#ig-header-search-box-input");
            await searchInput.FillAsync(nombre); // Rellena la barra de búsqueda con el nombre del juego

            // Simular la tecla Enter para buscar
            await searchInput.PressAsync("Enter");

            await Task.Delay(2000);

            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            //await Task.Delay(-1);
            // Le damos al botón de Sistemas
            IElementHandle spanButton = await page.WaitForSelectorAsync("span.select2-selection--single");
            await spanButton.ClickAsync(); // Abre el menú de sistemas

            // Esperar a que la lista de opciones esté disponible
            await page.WaitForSelectorAsync("ul.select2-results__options");

            // Selecciona la primera opción del menú desplegable de sistemas
            IElementHandle firstOption = await page.QuerySelectorAsync("ul.select2-results__options li.select2-results__option");
            await firstOption.ClickAsync(); // Hace clic en la primera opción(Selecciona los de PC)

            // Inicializa una lista para almacenar los juegos encontrados
            List<Juego> juegos = new List<Juego>();

            // Recoge todos los elementos que contienen información sobre los juegos
            IReadOnlyList<IElementHandle> juegosElements = await page.QuerySelectorAllAsync(".search.listing-items"); // Para encontrar cada producto
            // Selecciona el primer juego de la lista
            IElementHandle firts = juegosElements[0];

            // Obtiene los datos del primer juego utilizando la función GetProductAsync
            Juego juego = await GetProductAsync(firts);

            juegosDatos.Add(juego);

            Console.WriteLine(juego);
        }

        return juegosDatos;
        // await Task.Delay(-1);
    }
}
