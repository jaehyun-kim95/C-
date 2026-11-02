
// ===== 1. appsettins.json 읽기 =====
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Data;

string env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettins.{env}.json",optional: true)
    .AddEnvironmentVariables()
    .Build();

string? sourceFolder = configuration["Organizer:SourceFolder"];
bool isDryRun = configuration.GetValue<bool>("Organizer:DryRun");
string? unknownFolder = configuration["Organizer:UnknownFolder"];

Console.WriteLine(sourceFolder);
Console.WriteLine(isDryRun);
Console.WriteLine(unknownFolder);

var options = LoadOptions(configuration);
if (options is null) return;

// ===== 3. 환경설정. ===== 
Console.WriteLine($"환경: {env}");
Console.WriteLine($"DryRun: {options.DryRun}");
Console.WriteLine($"폴더: {options.SourceFolder}");

// ===== 2. class 바인딩 =====
OrganizerOption? LoadOptions(IConfiguration config)
{
    var api = configuration.GetSection("Organizer").Get<OrganizerOption>();
    if (api is null)
    {
        Console.WriteLine("설정 오류: appsettings.json에 Organizer 섹션이 없습니다.");
        return null;
    }

    if (string.IsNullOrWhiteSpace(api.SourceFolder) || !Directory.Exists(api.SourceFolder))
    {
        Console.WriteLine($"설정 오류: 정리할 폴더가 없습니다 ({api.SourceFolder})");
        return null;
    }

    return api;
}

class OrganizerOption
{
    public string SourceFolder { get; set; } = "";
    public bool DryRun { get; set; } = true;
    public string UnknownFolder { get; set; } = "기타";
    public Dictionary<string, string[]> Rules { get; set; } = new();
}