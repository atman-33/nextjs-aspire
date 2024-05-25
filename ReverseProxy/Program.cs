using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// NOTE: ServiceDiscovery ��L���Ƃ���B
// AppHost �v���W�F�N�g�Ŏ������� Next.js �� WebAPI �v���W�F�N�g�ւ̎Q�Ƃ� SerivceDiscovery �ŉ����ł���悤�ɂȂ邪�A
// YARP �̏ꍇ�́A���ꂾ���ł́AServiceDiscovery ���ł��Ȃ��B
builder.AddServiceDefaults();

// NOTE: AddReverseProxy �� Yarp.ReverseProxy �p�b�P�[�W�Ɋ܂܂�Ă��� YARP ���g�p���邱�Ƃ��p�C�v���C���ɓK�p���邽�߂̃��\�b�h�B
// Add ������@Use ����̂��p�C�v���C���̊�{�Ȃ̂ŁAUse ����Ŏ������Ă���i����́AMap�` �� Use �ɑ�������j�B
builder.Services.AddReverseProxy()
    .LoadFromMemory(GetRoutes(), GetClusters())
    .AddServiceDiscoveryDestinationResolver();

var app = builder.Build();

app.MapDefaultEndpoints();

// NOTE: �p�C�v���C����Use
app.MapReverseProxy();

app.Run();

// GetRoutes ���\�b�h�ŐU�蕪���̃��[�����`
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

// GetClusters ���\�b�h�ŐU�蕪����̃p�X���`
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