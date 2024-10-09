using System;
using System.Diagnostics;
using Microsoft.Playwright;


namespace InstantGaming
{
    internal class Program
    {
        static async Task Main(string[] args) //Metodo async solo funciona con main task
        {
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
            IElementHandle? acceptButton = await page.QuerySelectorAsync("//button[text()='Aceptar todo']");
            if (acceptButton != null) await acceptButton.ClickAsync();

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



            await Task.Delay(-1);

            //-------------------------------CODIGO OK NO TOCAR-------------------------------
            //Correciones a implementar: Cookies a veces aparecen mas tarde







            //select2 - selection select2 - selection--single
            // //Clase producto con name url precio
            // // Recorremos la lista de productos y recolectamos los datos
            // List<Juego> products = new List<Juego>();
            // IReadOnlyList<IElementHandle> productElements = await page.QuerySelectorAllAsync("ul li.s-item"); // Para encontrar cada producto

            // //Iterar lista
            // foreach (IElementHandle productElement in productElements)
            // {
            //     try
            //     {
            //         Juego product = await GetProductAsync(productElement);
            //         products.Add(product);
            //         Console.WriteLine(product);
            //     }
            //     catch (Exception e)
            //     {

            //     }

            // }

            // // Con los datos recolectados, buscamos el producto más barato
            // //Juego cheapest = products.MinBy(p => p.Juego);
            //// Console.WriteLine($"La oferta más barata es: {cheapest}");

            // // Abrimos el navegador con la oferta más barata
            // ProcessStartInfo processInfo = new ProcessStartInfo()
            // {
            //     FileName = cheapest.Url,
            //     UseShellExecute = true
            // };
            // Process.Start(processInfo);

            //await Task.Delay(-1);



        }



    }
}
