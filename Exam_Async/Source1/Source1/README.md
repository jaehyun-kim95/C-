# 여행 가격 비교: async/await와 Task.WhenAll 실습

항공사 3곳, 호텔 3곳에 가격을 물어보고 가장 싼 항공 + 호텔 조합을 찾는 콘솔 프로그램입니다.
외부 업체 API 호출은 `Task.Delay`로 흉내 냈고, 단계마다 "느린 업체", "장애 난 업체" 같은 상황을 추가했습니다.

정답 코드 없이 요구사항과 기대 결과만 보고 직접 구조를 설계하며 진행했습니다.

## 데이터

| 업체 | 종류 | 응답 시간 | 가격 |
|---|---|---|---|
| 하늘항공 | 항공 | 2.0초 | 89,000 |
| 바다항공 | 항공 | 1.0초 | 95,000 |
| 구름항공 | 항공 | 3.0초 | 79,000 |
| 제주스테이 | 호텔 | 1.5초 | 120,000 |
| 한라호텔 | 호텔 | 2.5초 | 150,000 |
| 오름리조트 | 호텔 | 0.8초 | 응답 대신 예외 (서버 점검 중) |

```csharp
record Company(string Name, string Kind, int DelayMs, int? Price);   // 업체 정보
record Quote(string Name, int Price);                                // 조회 결과
```

업체 정보와 조회 결과는 역할이 달라서 타입을 나눴습니다.
처음에는 "가격이 없는 업체"를 `int?`로 표현할지, 실패 여부 필드를 따로 둘지 고민했고, `int?`로 가되 null이면 예외를 던지도록 했습니다.

## 단계별 결과

| 단계 | 방식 | 소요 시간 |
|---|---|---|
| 1 | 동기 (`Thread.Sleep`) | 약 6.0초 |
| 2 | async, `foreach` 안에서 순차 `await` | 약 6.0초 |
| 3 | 모두 시작 후 `Task.WhenAll` | 약 3.0초 |
| 4 | 항공·호텔 그룹을 순차로 / 동시에 | 약 5.5초 / 3.0초 |
| 5 | 실패한 업체는 제외하고 계속 | 약 3.0초 |
| 6 | 업체당 2.5초 넘으면 취소 | 약 2.5초 |

### 2단계: async로 바꿨는데 빨라지지 않은 이유

`foreach` 안에서 업체마다 `await`하면 앞 업체의 응답을 받은 뒤에야 다음 업체 조회를 **시작**합니다.
작업이 하나도 겹치지 않아 동기 방식과 같은 시간(각 응답 시간의 합)이 걸립니다.
async/await는 기다리는 동안 스레드를 놓아줄 뿐, 그것만으로 빨라지지는 않습니다.

### 3단계: WhenAll로 빨라진 이유

```csharp
var tasks = flights.Select(f => GetPriceAsync(f)).ToList();   // 세 조회를 먼저 모두 시작
int[] prices = await Task.WhenAll(tasks);                      // 한꺼번에 기다림
```

```
[0.0초] 하늘항공 조회 시작
[0.0초] 바다항공 조회 시작
[0.0초] 구름항공 조회 시작
[1.0초] 바다항공 응답
[2.0초] 하늘항공 응답
[3.0초] 구름항공 응답
```

"시작"과 "기다림"을 분리하니 전체 시간이 가장 느린 업체(3초) 기준으로 줄었습니다.
응답은 바다 → 하늘 → 구름 순서로 왔지만, `WhenAll`의 결과 배열은 **넘긴 순서**(하늘 → 바다 → 구름)로 나왔습니다.

### 4단계: 두 그룹을 동시에

```csharp
var flightTask = QueryAllAsync(flights);
var hotelTask  = QueryAllAsync(hotels);
await Task.WhenAll(flightTask, hotelTask);

var flightQuotes = await flightTask;   // 이미 끝난 Task라 즉시 결과를 꺼냄
```

이미 끝난 Task를 `await`하면 기다리지 않고 결과만 꺼낸다는 점을 이용했습니다.

### 5단계: 한 곳이 실패해도 계속

`WhenAll`은 Task 하나라도 실패하면 예외를 던지기 때문에, 오름리조트 하나 때문에 정상 응답한 호텔 가격까지 모두 잃었습니다.
업체 하나를 조회하는 단위에서 예외를 잡아 **실패 결과(null)로 바꿔 반환**하는 방식으로 해결했습니다.

```csharp
async Task<Quote?> TryGetPriceAsync(Company company, TimeSpan? timeout)
{
    try   { ... return new Quote(company.Name, price); }
    catch (InvalidOperationException ex) { Console.WriteLine(...); return null; }
}

var hotelQuotes = (await Task.WhenAll(hotelTasks)).OfType<Quote>().ToList();
```

`Where(q => q is not null)`는 null을 빼도 타입이 `Quote?`로 남아 nullable 경고가 나서,
null을 빼면서 타입도 `Quote`로 바꿔 주는 `OfType<Quote>()`를 썼습니다.

### 6단계: 너무 느린 업체는 취소

```csharp
using var cts = new CancellationTokenSource();
cts.CancelAfter(TimeSpan.FromMilliseconds(2500));          // 2.5초 후 자동 취소
int price = await GetPriceAsync(company, cts.Token);       // 안에서 Task.Delay(ms, token)
```

| 역할 | 위치 |
|---|---|
| 제한 시간 정하기 | 호출하는 쪽 (`TryGetPriceAsync`) |
| 취소되면 멈추기 (`Task.Delay(..., token)`) | 작업하는 쪽 (`GetPriceAsync`) |
| 시간 초과 처리 (`catch OperationCanceledException`) | 호출하는 쪽 |

가장 싼 구름항공(3초)이 제외되면서 항공 최저가가 하늘항공으로, 패키지 가격이 199,000원에서 209,000원으로 바뀌었습니다.
가장 싼 업체가 가장 느릴 때 기다릴지 포기할지는 기술이 아니라 서비스 정책의 문제라는 점도 생각해 보게 됐습니다.
한라호텔(2.5초)은 제한 시간과 같은 경계값이라 실행할 때마다 포함되거나 빠질 수 있습니다.

## Task를 만들 때 ToList()가 필요한 이유

이번 과제에서 가장 크게 배운 부분입니다.

```csharp
flights.Select(s => GetPriceAsync(s));                       // ① 결과를 버림
var tasks = flights.Select(s => GetPriceAsync(s));           // ② ToList 없음
var tasks = flights.Select(s => GetPriceAsync(s)).ToList();  // ③ ToList 있음
```

LINQ `Select`는 "이렇게 바꿔라"라는 **메모**만 만들고, 결과를 꺼낼 때 실행됩니다.

| 코드 | `GetPriceAsync` 호출 시점 |
|---|---|
| ① 결과를 버림 | **호출 안 됨** |
| ② `ToList()` 없음 | `tasks`를 순회할 **때마다** |
| ③ `ToList()` 있음 | 그 줄에서 **한 번만** |

처음에 ①처럼 써서 조회가 하나도 시작되지 않았고, 빈 `prices` 배열을 읽다가 `IndexOutOfRangeException`이 났습니다.

②로 바꾸면 `WhenAll`이 한 번 순회하면서 조회가 시작되어 겉보기엔 정상입니다.
하지만 그 뒤에 `tasks.Count()`를 호출하니 개수를 세려고 다시 순회하면서 **조회가 한 번 더 시작**됐습니다(`조회 시작` 로그가 세 줄 더 찍힘).

지연 실행 과제에서 "같은 쿼리를 여러 번 쓰면 검사도 여러 번 실행된다"를 확인했는데,
그때는 가벼운 조건 검사였지만 여기서는 외부 API 호출입니다.
결제 API라면 결제가 두 번, DB 조회라면 쿼리가 두 번 날아갈 수 있습니다.

> `Select`로 Task를 만들 때는 `.ToList()`로 즉시 실행해서 Task 목록을 확정한다.

## 작성하면서 실수한 것들

**1. 최저가를 조회하기 전에 계산함**
조회 메서드가 가격을 반환하지 않고 출력만 해서, 최저가를 `companies` 전체(호텔 포함)에서 미리 구하고 있었습니다.
구름항공이 전체에서도 가장 싸서 결과가 우연히 맞았습니다. 조회 결과를 `Quote` 목록에 모은 뒤 그 목록에서 최저가를 구하도록 바꿨습니다.

**2. 실패를 `return 0`으로 숨김**
가격이 null인 업체에 0을 반환했더니, 나중에 호텔 최저가로 0원이 뽑힐 구조였습니다. 응답하지 못한 업체는 예외를 던지도록 바꿨습니다.

**3. 복사한 코드에서 변수 이름을 안 바꿈**
2단계 최저가를 `results2`가 아니라 1단계의 `results`에서 구했고, 4단계 호텔 반복문은 `hotelList.Count`가 아니라 `flightList.Count`로 돌았습니다.
둘 다 우연히 개수나 값이 같아서 출력으로는 드러나지 않았습니다. 반복되는 출력 코드는 `PrintCheapest`, `PrintPackage` 메서드로 뺐습니다.

**4. `await` 없는 호출을 `try/catch`로 감쌈**
`await` 없이 호출하면 예외는 Task 안에 담겨 있다가 `await`하는 순간 던져지므로, 그 `try`로는 잡을 수 없었습니다.

**5. `Task.WhenAll()`에 아무것도 넘기지 않음**
빈 호출에 `await`도 없어서 조회 응답을 기다리지 않고 프로그램이 끝났습니다.

**6. 기다림이 끝난 뒤에 취소함**
6단계에서 처음에는 `Task.Delay`를 다 기다린 뒤 `Cancel()`을 호출했고, 응답 시간(`DelayMs`)을 미리 보고 취소 여부를 정했습니다.
취소는 기다리는 도중에 일어나야 하고, 실제 API는 응답 시간을 미리 알 수 없으니 "제한 시간이 지나면 취소"하는 타이머 토큰을 써야 했습니다.

**7. 출력 메서드 안에서 패키지 가격을 누적**
`PrintCheapest` 안에서 바깥 변수 `packPrice`에 가격을 더하게 했더니, 출력 메서드가 계산까지 맡게 됐습니다.
`PrintCheapest`가 최저가를 반환하게 하고, 패키지 가격은 호출하는 쪽에서 계산하도록 바꿨습니다.

## 실행

```bash
dotnet run
```

전체 실행에 약 30초가 걸립니다. 특정 단계만 보려면 `Program.cs` 위쪽 실행 부분에서 나머지 단계를 주석 처리하면 됩니다.

## 다음에 해 볼 것

- 7단계: `Task.WhenAny`로 응답이 오는 순서대로 바로 출력하기
- `SemaphoreSlim`으로 동시에 조회하는 업체 수 제한하기
- `Task.Delay` 대신 실제 공개 API를 `HttpClient`로 호출하기
