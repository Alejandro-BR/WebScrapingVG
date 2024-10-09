using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Playwright;


namespace InstantGaming;

internal class Program
{
    static async Task Main(string[] args) //Metodo async solo funciona con main task
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        // Necesario para instalar los navegadores
        Microsoft.Playwright.Program.Main(["install"]);

        using IPlaywright playwright = await Playwright.CreateAsync();

        BrowserTypeLaunchOptions options = new BrowserTypeLaunchOptions()
        {
            Headless = false // Se indica falso para poder ver el navegador
        };

        await using IBrowser browser = await playwright.Chromium.LaunchAsync(options); //limpia navegador
        await using IBrowserContext context = await browser.NewContextAsync();
        IPage page = await context.NewPageAsync();

        // Ir a la página de Instant Gaming
        await page.GotoAsync("https://www.instant-gaming.com/");

        
        // Aceptar condiciones de Instant Gaming
        IElementHandle acceptButton = await page.QuerySelectorAsync("//button[text()='Aceptar todo']");
        if (acceptButton != null) await acceptButton.ClickAsync();
        await Task.Delay(2000);//Espera de 2 milisegundos esperando cookies


        // Le damos al botón de buscar
        IElementHandle searchButton = await page.QuerySelectorAsync(".icon-search-input");
        await searchButton.ClickAsync();


        // Escribimos en la barra de búsqueda lo que queremos buscar
        IElementHandle searchInput = await page.QuerySelectorAsync("#ig-header-search-box-input");
        await searchInput.FillAsync("The Witcher 3");

        // Simular la tecla Enter para buscar
        await searchInput.PressAsync("Enter");


        // Le damos al botón de Sistemas
        IElementHandle spanButton = await page.WaitForSelectorAsync("span.select2-selection--single");
        await spanButton.ClickAsync(); 

        // Esperar a que la lista de opciones esté disponible
        await page.WaitForSelectorAsync("ul.select2-results__options");

        IElementHandle firstOption = await page.QuerySelectorAsync("ul.select2-results__options li.select2-results__option");
        await firstOption.ClickAsync();


        //-------------------------------CODIGO OK NO TOCAR-------------------------------
        

        // Clase producto con name url precio
        // Recorremos la lista de productos y recolectamos los datos
        List<Juego> juegos = new List<Juego>();
        IReadOnlyList<IElementHandle> juegosElements = await page.QuerySelectorAllAsync(".search.listing-items"); // Para encontrar cada producto
        IElementHandle firts = juegosElements[0];
        
        
        Juego juego = await GetProductAsync(firts);

        Console.WriteLine(juego);

        await Task.Delay(-1);
    }



    private static async Task<Juego> GetProductAsync(IElementHandle element)
    {
        // PRECIO
        IElementHandle priceElement = await
        element.QuerySelectorAsync(".information .price"); // Referencia le span con texto
        string priceRaw = await priceElement.InnerTextAsync(); // Coge el texto del span
        // Quitar el EUR
        priceRaw = priceRaw.Replace("€", "", StringComparison.OrdinalIgnoreCase);
        // Quitar los espacios al principio y al final de la cadena
        priceRaw = priceRaw.Trim();
        // Pasar a decimal
        decimal price = decimal.Parse(priceRaw);

        // Devolver el producto
        return new Juego("The Witcher 3: Wild Hunt", price);
    }
}
