using PuppeteerSharp;

async Task<object> nakdan(PuppeteerSharp.Page page, string input){
    await page.GoToAsync("http://www.nakdan.com/Nakdan.aspx");
    await page.TypeAsync("#txtAreaMain", input);
    await page.ClickAsync("#btnStartAll");
    var txtArea = await page.QuerySelectorAsync("#txtAreaMain");
    var txtAreaValue = await txtArea.GetPropertyAsync("value");
    var result = await txtAreaValue.JsonValueAsync();
    return result;
}

async Task<Object> huggingFace(PuppeteerSharp.Page page, string input){
    await page.GoToAsync("https://huggingface.co/spaces/malper/unikud", new NavigationOptions{
        WaitUntil = new WaitUntilNavigation[] { WaitUntilNavigation.Networkidle0},
    });
    await Task.Delay(5000);
    var frameElement = await page.QuerySelectorAsync("#iFrameResizer0");
    var frame = await frameElement.ContentFrameAsync();
    var frameContent = await frame.GetContentAsync();
    await frame.TypeAsync("textarea", input);
    await page.Keyboard.DownAsync("Control");
    await page.Keyboard.PressAsync("Enter");
    await page.Keyboard.UpAsync("Control");
    await frame.WaitForSelectorAsync(".stMarkdown");
    var markDown = await frame.QuerySelectorAsync(".stMarkdown");
    var pMarkDown = await markDown.QuerySelectorAsync("p");
    var txtAreaValue = await pMarkDown.GetPropertyAsync("innerText");
    var result = await txtAreaValue.JsonValueAsync();
    return result;
}

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

var app = builder.Build();
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);

var client = new HttpClient();
var response = await client.GetStringAsync("http://www.nakdan.com/Nakdan.aspx");

app.MapGet("/", () => "Test success");

app.MapGet("/nikkud", async(string input) => 
    {
        object result = input;
        var options = new LaunchOptions { 
            Headless = true,
            Args = new[]{
                "--disable-web-security",
                "--disable-features=IsolateOrigins,site-per-process",
        }};

        using (var browserFetcher = new BrowserFetcher())
        {
            await browserFetcher.DownloadAsync();
            await using (var browser = await Puppeteer.LaunchAsync(options))
            {
                await using (var page = await browser.NewPageAsync())
                {
                    var amountWords = input.Split(' ').Length;
                    if (amountWords > 1){
                        result = await nakdan(page, input);
                        
                        var toString = result.ToString();
                        if (toString == input){
                            result = await huggingFace(page, input);
                        }
                    }
                    else if (amountWords == 1) {
                        result = await huggingFace(page, input);
                    }
                }
            }
        }

        return result.ToString();
    }
);

app.Run();
