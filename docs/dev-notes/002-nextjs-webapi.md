# Next.js から WebAPIを叩く

## ステップ

### WebApiのエンドポイントを変更

Next.js の実装をする前に、少しだけ WebAPI を修正します。WebAPI のエンドポイントは必ず /api の下になるようにします。WebAPI プロジェクトの Program.cs を次のように修正します。

e.g.  

```cs
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};


- app.MapGet("/weatherforecast", () =>
+ app.MapGet("api/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
```

### Server Components

`frontend\src\app\server\page.tsx`

```tsx
const getData = async () => {

    const apiServer = process.env['services__webapi__https__0'] ?? process.env['services__webapi__http__0'];
    const weatherData: Response = await fetch(`${apiServer}/api/weatherforecast`, { cache: 'no-cache' });

    if (!weatherData.ok) {
        throw new Error('Failed to fetch data.');
    }

    const data = await weatherData.json();

    return data
}

const Page = async () => {
    const data = await getData()

    return <main>{JSON.stringify(data)}</main>
}

export default Page
```

> services__webapi__https__0 と services__webapi__http__0 という　環境変数の値に WebAPI のホスト名が格納されている。
