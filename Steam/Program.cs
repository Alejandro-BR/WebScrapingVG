using Microsoft.Playwright;
using System.Diagnostics;

namespace Steam;

internal class Program
{
    public static async Task Main()
    {
        // Instalar los navegadores
        Microsoft.Playwright.Program.Main(["install"]);

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

        // Aceptar cookies si aparece el boton
        IElementHandle? acceptButton = await page.QuerySelectorAsync(".btn_blue_steamui.btn_medium");
        if (acceptButton != null) await acceptButton.ClickAsync();

        // Escribir en la barra de busqueda
        IElementHandle searchInput = await page.QuerySelectorAsync("#store_nav_search_term");
        await searchInput.FillAsync("The Witcher 3: Wild Hunt");

        // Simular la tecla Enter para buscar
        await searchInput.PressAsync("Enter");

        // Mantener el navegador abierto
        await Task.Delay(-1);
    }
}
