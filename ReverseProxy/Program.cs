using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// NOTE: ServiceDiscovery を有効とする。
// AppHost プロジェクトで実装した Next.js と WebAPI プロジェクトへの参照を SerivceDiscovery で解決できるようになるが、
// YARP の場合は、これだけでは、ServiceDiscovery ができない。
builder.AddServiceDefaults();

// NOTE: AddReverseProxy は Yarp.ReverseProxy パッケージに含まれている YARP を使用することをパイプラインに適用するためのメソッド。
// Add したら　Use するのがパイプラインの基本なので、Use を後で実装している（今回は、Map～ が Use に相当する）。
builder.Services.AddReverseProxy()
    .LoadFromMemory(GetRoutes(), GetClusters())
    .AddServiceDiscoveryDestinationResolver();

var app = builder.Build();

app.MapDefaultEndpoints();

// NOTE: パイプラインのUse
app.MapReverseProxy();

app.Run();

// GetRoutes メソッドで振り分けのルールを定義
RouteConfig[] GetRoutes()
{
    return
    [
        new RouteConfig
        {
            RouteId = "Route1",
            ClusterId = "default",
            Match = new RouteMatch { Path = "{**catch-all}" }
        },
        new RouteConfig
        {
            RouteId = "Route2",
            ClusterId = "api",
            Match = new RouteMatch { Path = "/api/{*any}" }
        },
    ];
}

// GetClusters メソッドで振り分け先のパスを定義
ClusterConfig[] GetClusters()
{
    return
    [
        new ClusterConfig
        {
            ClusterId = "default",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                { "destination1", new DestinationConfig { Address = "http://frontend" } },
            }
        },
        new ClusterConfig
        {
            ClusterId = "api",
            Destinations = new Dictionary<string, DestinationConfig>
            {
                { "destination2", new DestinationConfig { Address =  "http://webapi", Host = "localhost" } },
            }
        },
    ];
}