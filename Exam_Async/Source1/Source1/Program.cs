using System.Diagnostics;

// ===== 데이터 =====
// 외부 업체 API를 흉내 낸 데이터. DelayMs = 응답 시간, Price가 null이면 응답 대신 예외 발생
var companies = new List<Company>
{
    new("하늘항공",   "항공", 2000,  89_000),
    new("바다항공",   "항공", 1000,  95_000),
    new("구름항공",   "항공", 3000,  79_000),
    new("제주스테이", "호텔", 1500, 120_000),
    new("한라호텔",   "호텔", 2500, 150_000),
    new("오름리조트", "호텔",  800, null),      // 5단계부터 사용: 서버 점검 중
};

// 여러 번 순회하므로 ToList()로 조회
var flights = companies.Where(c => c.Kind == "항공").ToList();
var hotels = companies.Where(c => c.Kind == "호텔" && c.Price is not null).ToList(); // 1~4단계용 (정상 호텔만)
var allHotels = companies.Where(c => c.Kind == "호텔").ToList();                         // 5단계부터 (오름리조트 포함)

// ===== 실행 =====
PrintHeader("1단계: 동기 방식");
Step1Sync();

PrintHeader("2단계: async 순차 await");
await Step2AsyncSequential();

PrintHeader("3단계: Task.WhenAll 동시 조회");
await Step3WhenAll();

PrintHeader("4단계: 항공·호텔 그룹 동시 조회");
await Step4FlightAndHotel();

PrintHeader("5단계: 실패한 업체는 제외하고 계속");
await RunSearchAsync(allHotels, timeout: null);

PrintHeader("6단계: 2.5초 넘으면 제외 (타임아웃)");
await RunSearchAsync(allHotels, timeout: TimeSpan.FromMilliseconds(2500));


// ===== 1단계: 동기 방식 (기준 측정) =====
void Step1Sync()
{
    var sw = Stopwatch.StartNew();
    var results = new List<Quote>();

    foreach (var flight in flights)
    {
        int price = GetPriceSync(flight);
        Console.WriteLine($"[동기] {flight.Name}: {price:N0}원");
        results.Add(new Quote(flight.Name, price));
    }

    PrintCheapest("항공", results);
    PrintElapsed(sw);
}

// ===== 2단계: async로 바꾸기 (순차 await) =====
// 앞 업체의 응답을 받은 뒤에야 다음 업체 조회를 시작하므로 1단계와 같은 시간이 걸린다
async Task Step2AsyncSequential()
{
    var sw = Stopwatch.StartNew();
    var results = new List<Quote>();

    foreach (var flight in flights)
    {
        int price = await GetPriceAsync(flight);
        Console.WriteLine($"[비동기] {flight.Name}: {price:N0}원");
        results.Add(new Quote(flight.Name, price));
    }

    PrintCheapest("항공", results);
    PrintElapsed(sw);
}

// ===== 3단계: Task.WhenAll로 동시 조회 =====
// 먼저 모두 시작하고(ToList), 한꺼번에 기다린다(WhenAll)
async Task Step3WhenAll()
{
    var sw = Stopwatch.StartNew();

    var tasks = flights.Select(f => GetPriceWithLogAsync(f, sw)).ToList();   // 이 줄에서 세 조회가 시작됨(tolist) 
    int[] prices = await Task.WhenAll(tasks);                                // 결과는 넘긴 순서대로

    var results = flights.Zip(prices, (f, p) => new Quote(f.Name, p)).ToList();
    foreach (var q in results)
        Console.WriteLine($"[동시] {q.Name}: {q.Price:N0}원");

    PrintCheapest("항공", results);
    PrintElapsed(sw);
}

// ===== 4단계: 항공과 호텔을 동시에 =====
async Task Step4FlightAndHotel()
{
    // 비교용: 항공 그룹이 끝난 뒤 호텔 그룹 시작
    var sw = Stopwatch.StartNew();
    await QueryAllAsync(flights);
    await QueryAllAsync(hotels);
    sw.Stop();
    Console.WriteLine($"그룹별 순차 (항공 → 호텔): 약 {sw.Elapsed.TotalSeconds:F1}초");

    // 두 그룹을 먼저 모두 시작한 뒤 함께 기다림
    sw.Restart();
    var flightTask = QueryAllAsync(flights);
    var hotelTask = QueryAllAsync(hotels);
    await Task.WhenAll(flightTask, hotelTask);

    var flightQuotes = await flightTask;   // 이미 끝난 Task라 즉시 결과를 꺼냄
    var hotelQuotes = await hotelTask;

    PrintPackage(flightQuotes, hotelQuotes);
    sw.Stop();
    Console.WriteLine($"그룹 동시 진행: 약 {sw.Elapsed.TotalSeconds:F1}초");
}

// ===== 5·6단계: 실패·시간 초과 업체를 제외하고 검색 =====
async Task RunSearchAsync(List<Company> hotelList, TimeSpan? timeout)
{
    var sw = Stopwatch.StartNew();

    var flightTasks = flights.Select(c => TryGetPriceAsync(c, timeout)).ToList();
    var hotelTasks = hotelList.Select(c => TryGetPriceAsync(c, timeout)).ToList();

    // 실패한 업체는 null로 돌아오므로 OfType<Quote>()로 제외 (타입도 Quote?에서 Quote로 바뀜)
    var flightQuotes = (await Task.WhenAll(flightTasks)).OfType<Quote>().ToList();
    var hotelQuotes = (await Task.WhenAll(hotelTasks)).OfType<Quote>().ToList();

    PrintPackage(flightQuotes, hotelQuotes);
    PrintElapsed(sw);
}


// ===== 업체 조회 =====
int GetPriceSync(Company company)
{
    Thread.Sleep(company.DelayMs);
    if (company.Price is null)
        throw new InvalidOperationException("서버 점검 중");
    return company.Price.Value;
}

async Task<int> GetPriceAsync(Company company, CancellationToken token = default)
{
    await Task.Delay(company.DelayMs, token);   // 토큰이 취소되면 기다리는 도중에 OperationCanceledException
    if (company.Price is null)
        throw new InvalidOperationException("서버 점검 중");
    return company.Price.Value;
}

// 3단계용: 시작·응답 시각을 함께 출력
async Task<int> GetPriceWithLogAsync(Company company, Stopwatch sw)
{
    Console.WriteLine($"[{sw.Elapsed.TotalSeconds:F1}초] {company.Name} 조회 시작");
    int price = await GetPriceAsync(company);
    Console.WriteLine($"[{sw.Elapsed.TotalSeconds:F1}초] {company.Name} 응답");
    return price;
}

// 5·6단계용: 예외를 잡아 실패 결과(null)로 바꿔서, 한 업체가 실패해도 WhenAll 전체가 실패하지 않게 함
async Task<Quote?> TryGetPriceAsync(Company company, TimeSpan? timeout)
{
    using var cts = new CancellationTokenSource();
    if (timeout is not null)
        cts.CancelAfter(timeout.Value);         // 제한 시간이 지나면 자동 취소

    try
    {
        int price = await GetPriceAsync(company, cts.Token);
        return new Quote(company.Name, price);
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine($"조회 실패: {company.Name} (시간 초과)");
        return null;
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"조회 실패: {company.Name} ({ex.Message})");
        return null;
    }
}


// ===== 출력 =====
Quote? PrintCheapest(string label, List<Quote> quotes)
{
    var cheapest = quotes.MinBy(q => q.Price);
    Console.WriteLine(cheapest is null
        ? $"{label} 최저가: 없음"
        : $"{label} 최저가: {cheapest.Name} {cheapest.Price:N0}원");
    return cheapest;
}

void PrintPackage(List<Quote> flightQuotes, List<Quote> hotelQuotes)
{
    var cheapFlight = PrintCheapest("항공", flightQuotes);
    var cheapHotel = PrintCheapest("호텔", hotelQuotes);

    Console.WriteLine(cheapFlight is not null && cheapHotel is not null
        ? $"패키지 최저가: {cheapFlight.Price + cheapHotel.Price:N0}원"
        : "패키지 구성 불가");
}

async Task<List<Quote>> QueryAllAsync(List<Company> list)
{
    var tasks = list.Select(c => GetPriceAsync(c)).ToList();
    int[] prices = await Task.WhenAll(tasks);
    return list.Zip(prices, (c, p) => new Quote(c.Name, p)).ToList();
}

void PrintHeader(string title) => Console.WriteLine($"\n===== {title} =====");

void PrintElapsed(Stopwatch sw)
{
    sw.Stop();
    Console.WriteLine($"소요 시간: 약 {sw.Elapsed.TotalSeconds:F1}초");
}


// ===== 타입 =====
record Company(string Name, string Kind, int DelayMs, int? Price);   // 업체 정보
record Quote(string Name, int Price);                                // 조회 결과