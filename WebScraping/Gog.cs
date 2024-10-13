using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraping;

internal class Gog
{
    /**
     * Metodo main
     *
     * @param {string[]} nombresJuegos - Array de nombres que hay que buscar
     * @return {Promise<List<Juego>>} - Lista de los juegos
     */
    public static async Task<List<Juego>> GetJuegos(string[] nombresJuegos)
    {
        Console.WriteLine("\nGOG --> Lista de juegos:\n");

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
        element.QuerySelectorAsync(".final-value"); // Referencia le span con texto
        string priceRaw = await priceElement.InnerTextAsync(); // Coge el precio del span

        // NOMBRE
        IElementHandle nameElement = await
        element.QuerySelectorAsync(".product-tile__title"); // Referencia le span con texto
        string textName = await nameElement.InnerTextAsync(); // Coge el texto del span

        // Quitar el EUR
        priceRaw = priceRaw.Replace("€", "", StringComparison.OrdinalIgnoreCase);

        // Cambiar el punto por una coma
        priceRaw = priceRaw.Replace(".", ",", StringComparison.OrdinalIgnoreCase);

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

        // DOM
        const string BOTON_COOKIES = "#CybotCookiebotDialogBodyLevelButtonLevelOptinAllowAll";
        const string BARRA_BUSQUEDA = ".menu-search-input__field";
        const string LUPA = ".menu-link.menu-link--last.menu-link--search.menu-link--icon";
        const string BUSQUEDA_CATALOGO = ".search__input";
        const string RESULTADO = ".product-tile";

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
        await page.GotoAsync("https://www.gog.com/es/");

        // Aceptar condiciones de Instant Gaming (cookies)
        // Esperar a que el botón de aceptar cookies esté visible
        await page.WaitForSelectorAsync(BOTON_COOKIES);
        IElementHandle acceptButton = await page.QuerySelectorAsync(BOTON_COOKIES);

        // Verificar si se encontró el botón y hacer clic en él
        if (acceptButton != null)
        {
            await acceptButton.ClickAsync();
        }

        // Le damos al botón de buscar del encabezado
        IElementHandle botonLupa = await page.QuerySelectorAsync(LUPA);
        await botonLupa.ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        bool primeraVez = true;

        foreach (var nombreJ in nombresJuegos)
        {
            if (primeraVez)
            {
                primeraVez = false;
                // Escribir en la barra de busqueda
                IElementHandle searchInput = await page.QuerySelectorAsync(BARRA_BUSQUEDA);
                await searchInput.FillAsync(nombreJ);

                // Simular la tecla Enter para buscar
                await searchInput.PressAsync("Enter");
                await Task.Delay(3000);

            }
            else
            {
                // Escribir en la barra de busqueda
                IElementHandle searchInput = await page.QuerySelectorAsync(BUSQUEDA_CATALOGO);
                await searchInput.FillAsync(nombreJ);

                // Simular la tecla Enter para buscar
                await searchInput.PressAsync("Enter");
                await Task.Delay(3000);
            }


            // Recorremos la lista de productos y recolectamos los datos
            List<Juego> juegos = new List<Juego>();
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            IReadOnlyList<IElementHandle> juegosElements = await page.QuerySelectorAllAsync(RESULTADO);
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            IElementHandle first = juegosElements[0];
            Juego juego = await GetProductAsync(first);

            juegosDatos.Add(juego);

            Console.WriteLine(juego);
        }

        return juegosDatos;
    }
}
