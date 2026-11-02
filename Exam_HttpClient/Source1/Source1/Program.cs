using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

// ===== 공용 객체 =====
// HttpClient는 요청마다 만들지 않고 하나를 재사용한다 (매번 만들면 연결이 바닥날 수 있음)
// JsonSerializerOptions도 내부 캐시가 있어 하나만 만들어 재사용한다
var http = new HttpClient();
var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

// ===== 실행 =====
PrintHeader("1단계: 원문 그대로 받기");
await Step1RawJson();

PrintHeader("2단계: 지오코딩 응답을 타입으로 받기");
await Step2Geocoding();

PrintHeader("3단계: 좌표로 날씨 받기");
await Step3Weather();

PrintHeader("4단계: 도시 이름 하나로 끝까지");
await Step4SingleCity();

PrintHeader("5단계: 여러 도시를 동시에");
await Step5MultipleCities();

PrintHeader("6단계: 네트워크 오류 처리");
await Step6Errors();


// ===== 1단계 =====
async Task Step1RawJson()
{
    string json = await http.GetStringAsync("https://geocoding-api.open-meteo.com/v1/search?name=Seoul&count=1&language=ko");
    Console.WriteLine(json);
}

// ===== 2단계 =====
async Task Step2Geocoding()
{
    foreach (var city in new[] { "Seoul", "Asdfghjk" })
    {
        Geo? geo = await FindCityAsync(city);
        Console.WriteLine(geo is null
            ? $"{city}: 찾지 못함"
            : $"{geo.Name} ({geo.Country}) 위도 {geo.Latitude:F2} / 경도 {geo.Longitude:F2}");
    }
}

// ===== 3단계 =====
async Task Step3Weather()
{
    Geo? geo = await FindCityAsync("Seoul");
    if (geo is null) { Console.WriteLine("도시를 찾지 못함"); return; }

    Forecast? forecast = await GetForecastAsync(geo.Latitude, geo.Longitude);
    if (forecast is null) { Console.WriteLine("날씨 응답 형식 오류"); return; }

    Console.WriteLine($"[{geo.Name}] 현재 {forecast.Current.Temperature:F1}°C, 풍속 {forecast.Current.WindSpeed:F1}km/h");

    // 날짜·최고·최저가 각각 다른 배열로 오므로 같은 위치끼리 짝지음
    var days = forecast.Daily.Time.Zip(forecast.Daily.MaxTemperature, forecast.Daily.MinTemperature);
    foreach (var (date, max, min) in days)
        Console.WriteLine($"  {date:MM/dd}  최고 {max:F1}°C / 최저 {min:F1}°C");
}

// ===== 4단계 =====
// 지오코딩 → 날씨는 앞의 결과(좌표)가 뒤의 입력이라 순서대로 호출해야 한다
async Task Step4SingleCity()
{
    WeatherResult result = await GetWeatherByCityAsync("Seoul");
    Console.WriteLine(FormatResult(result));
}

// ===== 5단계 =====
// 도시끼리는 서로 의존하지 않으므로 동시에 조회할 수 있다
async Task Step5MultipleCities()
{
    var cities = new[] { "Seoul", "Busan", "Jeju", "Tokyo", "Asdfghjk" };

    // 순차
    var sw = Stopwatch.StartNew();
    foreach (var city in cities)
        await GetWeatherByCityAsync(city);
    double sequential = sw.Elapsed.TotalSeconds;

    // 동시 (ToList로 즉시 시작 → WhenAll로 한꺼번에 기다림)
    sw.Restart();
    var tasks = cities.Select(c => GetWeatherByCityAsync(c)).ToList();
    WeatherResult[] results = await Task.WhenAll(tasks);
    double concurrent = sw.Elapsed.TotalSeconds;

    Console.WriteLine("도시      현재 기온   풍속        내일 최고 / 최저");
    foreach (var r in results.Where(r => r.Error is null))
        Console.WriteLine(FormatResult(r));

    foreach (var r in results.Where(r => r.Error is not null))
        Console.WriteLine($"조회 실패: {r.City} ({r.Error})");

    Console.WriteLine($"순차: 약 {sequential:F1}초 / 동시: 약 {concurrent:F1}초");
}

// ===== 6단계 =====
// 세 가지 오류는 서로 가리지 않도록 하나씩 따로 테스트한다
// (시간 제한과 잘못된 도메인을 같이 걸면 시간 초과가 먼저 나서 도메인 오류를 볼 수 없음)
async Task Step6Errors()
{
    // 연결 실패: 존재하지 않는 도메인 → 서버에 도달하지 못해 StatusCode가 null
    var connection = await GetWeatherByCityAsync("Seoul", weatherBaseUrl: "https://api.open-meteo.comx");
    Console.WriteLine($"[도메인 오류] {FormatResult(connection)}");

    // 요청 오류: 잘못된 위도 → 서버가 400으로 응답
    var badRequest = await GetWeatherByCityAsync("Seoul", latitudeOverride: 999);
    Console.WriteLine($"[잘못된 위도] {FormatResult(badRequest)}");

    // 시간 초과: 1ms 제한
    using var cts = new CancellationTokenSource(1);
    var timeout = await GetWeatherByCityAsync("Seoul", token: cts.Token);
    Console.WriteLine($"[1ms 제한] {FormatResult(timeout)}");
}


// ===== API 호출 =====
async Task<Geo?> FindCityAsync(string city, CancellationToken token = default)
{
    string json = await http.GetStringAsync($"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(city)}&count=1&language=ko", token);

    // 없는 도시는 응답에 "results"가 아예 없으므로 null
    JsonArray? results = JsonNode.Parse(json)?["results"]?.AsArray();
    if (results is null || results.Count == 0)
        return null;

    return results[0].Deserialize<Geo>(jsonOptions);
}

async Task<Forecast?> GetForecastAsync(double latitude, double longitude,
                                       string weatherBaseUrl = "https://api.open-meteo.com",
                                       CancellationToken token = default)
{
    // 숫자를 URL에 넣을 때는 지역 설정과 관계없이 소수점이 '.'이 되도록 InvariantCulture 사용
    string lat = latitude.ToString("F2", CultureInfo.InvariantCulture);
    string lon = longitude.ToString("F2", CultureInfo.InvariantCulture);

    string json = await http.GetStringAsync(
        $"{weatherBaseUrl}/v1/forecast?latitude={lat}&longitude={lon}" +
        "&current=temperature_2m,wind_speed_10m" +
        "&daily=temperature_2m_max,temperature_2m_min" +
        "&timezone=Asia%2FSeoul&forecast_days=3", token);

    JsonNode? node = JsonNode.Parse(json);
    CurrentTemp? current = node?["current"]?.Deserialize<CurrentTemp>(jsonOptions);
    Daily? daily = node?["daily"]?.Deserialize<Daily>(jsonOptions);

    if (current is null || daily is null)
        return null;

    return new Forecast(current, daily);
}

// 도시 하나를 끝까지 조회. 예외를 잡아 실패 결과로 바꾸므로 WhenAll에서 한 도시가 실패해도 나머지는 유지된다
// weatherBaseUrl, latitudeOverride는 6단계 오류 테스트용
async Task<WeatherResult> GetWeatherByCityAsync(string city,
                                                string weatherBaseUrl = "https://api.open-meteo.com",
                                                double? latitudeOverride = null,
                                                CancellationToken token = default)
{
    try
    {
        Geo? geo = await FindCityAsync(city, token);
        if (geo is null)
            return new WeatherResult(city, "찾지 못함");

        Forecast? forecast = await GetForecastAsync(latitudeOverride ?? geo.Latitude, geo.Longitude, weatherBaseUrl, token);
        if (forecast is null)
            return new WeatherResult(city, "응답 형식 오류");

        return new WeatherResult(geo.Name, null,
                                 forecast.Current.Temperature,
                                 forecast.Current.WindSpeed,
                                 forecast.Daily.MaxTemperature[1],   // [1] = 내일
                                 forecast.Daily.MinTemperature[1]);
    }
    catch (HttpRequestException ex) when (ex.StatusCode is null)   // 서버에 도달하지 못함 (DNS, 연결)
    {
        return new WeatherResult(city, "연결 실패");
    }
    catch (HttpRequestException ex)                                // 서버가 실패 상태 코드로 응답
    {
        return new WeatherResult(city, $"요청 오류 ({(int?)ex.StatusCode})");
    }
    catch (OperationCanceledException)                             // 시간 초과 (TaskCanceledException 포함)
    {
        return new WeatherResult(city, "시간 초과");
    }
    catch (JsonException)
    {
        return new WeatherResult(city, "응답 형식 오류");
    }
}


// ===== 출력 =====
string FormatResult(WeatherResult r) => r.Error is null
    ? $"{r.City,-6}  {r.Temperature,6:F1}°C  {r.WindSpeed,6:F1}km/h   {r.TomorrowMax:F1} / {r.TomorrowMin:F1}"
    : $"{r.City}: {r.Error}";

void PrintHeader(string title) => Console.WriteLine($"\n===== {title} =====");


// ===== 타입 =====
// 지오코딩: 필요한 필드만 선언하면 나머지는 무시된다. Country는 없을 수 있어 nullable
record Geo(string Name, double Latitude, double Longitude, string? Country);

// 날씨: temperature_2m처럼 C# 이름으로 쓰기 어려운 필드는 JsonPropertyName으로 지정
record CurrentTemp(DateTime Time,
                   [property: JsonPropertyName("temperature_2m")] double Temperature,
                   [property: JsonPropertyName("wind_speed_10m")] double WindSpeed);

record Daily(List<DateTime> Time,
             [property: JsonPropertyName("temperature_2m_max")] List<double> MaxTemperature,
             [property: JsonPropertyName("temperature_2m_min")] List<double> MinTemperature);

record Forecast(CurrentTemp Current, Daily Daily);

// 도시 하나의 조회 결과. Error가 null이면 성공
record WeatherResult(string City, string? Error,
                     double Temperature = 0, double WindSpeed = 0,
                     double TomorrowMax = 0, double TomorrowMin = 0);