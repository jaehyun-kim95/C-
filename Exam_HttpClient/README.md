# 도시별 날씨 조회: HttpClient와 System.Text.Json 실습

공개 날씨 API(Open-Meteo, API 키 불필요)를 `HttpClient`로 호출하고 `System.Text.Json`으로 역직렬화해 출력하는 콘솔 프로그램입니다.
도시 이름으로 좌표를 찾는 API와 좌표로 날씨를 조회하는 API를 이어서 호출하고,
여러 도시를 동시에 조회하며 실제 네트워크 오류를 구분해 처리했습니다.

Delphi에서는 `TNetHTTPClient`로 받은 JSON을 `TJSONObject`에서 필드 하나씩 꺼내 썼는데,
C#에서는 JSON 구조와 같은 record를 정의하면 자동으로 채워진다는 점이 가장 큰 차이였습니다.

## 사용한 API

| API | 용도 |
|---|---|
| `geocoding-api.open-meteo.com/v1/search?name=Seoul` | 도시 이름 → 좌표 |
| `api.open-meteo.com/v1/forecast?latitude=..&longitude=..` | 좌표 → 현재 날씨, 3일 예보 |

## 단계

| 단계 | 내용 |
|---|---|
| 1 | `GetStringAsync`로 응답 원문 출력 |
| 2 | 지오코딩 응답을 record로 역직렬화, 없는 도시 처리 |
| 3 | 날씨 응답 역직렬화, 날짜·최고·최저 배열을 `Zip`으로 짝지어 출력 |
| 4 | 도시 이름 하나로 지오코딩 → 날씨까지 |
| 5 | 5개 도시 동시 조회, 순차와 시간 비교 |
| 6 | 연결 실패 / 요청 오류(400) / 시간 초과 구분 처리 |

## 역직렬화

```csharp
record Geo(string Name, double Latitude, double Longitude, string? Country);

record CurrentTemp(DateTime Time,
                   [property: JsonPropertyName("temperature_2m")] double Temperature,
                   [property: JsonPropertyName("wind_speed_10m")] double WindSpeed);
```

- JSON은 `latitude`(소문자), C#은 `Latitude`(대문자)라서 `PropertyNameCaseInsensitive = true`로 맞췄습니다.
- `temperature_2m`처럼 C# 이름으로 쓰기 어려운 필드는 `JsonPropertyName`으로 지정했습니다.
- 응답 전체가 아니라 필요한 필드만 선언했습니다. 선언하지 않은 필드는 무시됩니다.

지오코딩 응답은 `{ "results": [ {...} ] }` 형태인데, 감싸는 타입을 따로 만들지 않고
`JsonNode`로 `results` 배열만 꺼낸 뒤 첫 항목을 역직렬화했습니다.
없는 도시는 `results`가 응답에 아예 없어서, 이 방식이 null 처리도 간단했습니다.

```csharp
JsonArray? results = JsonNode.Parse(json)?["results"]?.AsArray();
if (results is null || results.Count == 0) return null;
return results[0].Deserialize<Geo>(jsonOptions);
```

## 순서대로 vs 동시에

- **지오코딩 → 날씨**: 날씨 조회에 필요한 좌표가 지오코딩 결과라서 **순서대로** 호출해야 합니다.
- **도시끼리**: 서로 의존하지 않으므로 **동시에** 조회할 수 있습니다.

> 의존 관계가 있으면 순차, 없으면 동시

5개 도시를 동시에 조회하니 순차보다 확실히 빨라졌습니다. 실제 네트워크라 시간은 실행할 때마다 다릅니다.

## 오류 구분

`Task.Delay`로 흉내 냈던 이전 과제와 달리, 실제 네트워크는 여러 이유로 실패합니다.

| 상황 | 예외 | 구분 방법 |
|---|---|---|
| 도메인·연결 오류 | `HttpRequestException` | `StatusCode`가 **null** (서버에 도달 못 함) |
| 잘못된 요청 (위도 999) | `HttpRequestException` | `StatusCode`가 **400** |
| 시간 초과 | `TaskCanceledException` | `OperationCanceledException`으로 잡음 |

```csharp
catch (HttpRequestException ex) when (ex.StatusCode is null) { ... "연결 실패" }
catch (HttpRequestException ex)                               { ... "요청 오류" }
catch (OperationCanceledException)                            { ... "시간 초과" }
```

같은 예외 타입을 상황별로 나누려고 `when` 예외 필터를 처음 써 봤습니다.
예외는 다시 던지지 않고 실패 결과(`WeatherResult`의 `Error`)로 바꿔 반환해서,
한 도시가 실패해도 `WhenAll`로 조회한 나머지 도시 결과는 유지되게 했습니다.

## 작성하면서 실수한 것들

**1. 두 가지 오류를 동시에 테스트함**
도메인 오류가 잡히지 않아서 한참 헤맸는데, 같은 요청에 1ms 시간 제한까지 걸어 두고 있었습니다.
DNS 조회보다 1ms 타이머가 먼저 끝나서 항상 "시간 초과"만 나왔습니다. 오류 테스트는 하나씩 따로 해야 했습니다.

**2. HttpClient를 요청마다 새로 만듦**
`using var client = new HttpClient()`를 메서드마다 넣었습니다. `IDisposable`이라 `using`이 맞다고 생각했는데,
HttpClient는 예외적으로 하나를 만들어 재사용해야 합니다. 요청마다 만들면 요청이 많을 때 연결(소켓)이 바닥납니다.

**3. try가 두 번째 호출만 감쌈**
날씨 호출만 `try`로 감싸서, 지오코딩 단계에서 오류가 나면 예외가 그대로 나가 `WhenAll` 전체가 실패할 구조였습니다.

**4. 같은 조회 코드를 두 벌 만듦**
6단계용 메서드를 따로 만들어 오류 처리를 그쪽에만 넣었더니, 5단계(여러 도시)는 오류를 구분하지 못했습니다.
하나의 메서드로 합치고 테스트용 값만 매개변수로 바꿔 넣게 했습니다.

**5. 예외를 새로 만들어 다시 던짐**
`throw new HttpRequestException(ex.Message)`로 다시 던지면 원래 예외의 내부 정보와 발생 위치가 사라집니다.
다시 던질 거라면 `throw;`를 쓰고, 이번처럼 처리할 수 있으면 결과로 바꿔 반환하는 쪽이 맞았습니다.

**6. null을 확인하고도 빠져나가지 않음**
`if (geo is null) Console.WriteLine("찾지 못함");` 다음 줄에서 `geo.Latitude`를 읽어 `NullReferenceException`이 날 구조였습니다. `return`이 필요했습니다.

**7. 응답 필드를 전부 선언함**
처음에는 지오코딩 응답의 필드 13개를 모두 record에 넣었습니다.
`admin1`처럼 도시에 따라 응답에 없는 필드가 있는데 `string`(null 불가)으로 선언해서, 타입과 실제 값이 어긋날 수 있었습니다.
역직렬화는 이름이 안 맞거나 값이 없어도 오류를 내지 않고 기본값이나 null을 넣기 때문에 더 조심해야 했습니다.

## 실행

```bash
dotnet run
```

날씨 값은 실제 데이터라 실행할 때마다 다릅니다.

## 다음에 해 볼 것

- `System.Net.Http.Json`의 `GetFromJsonAsync<T>`로 바꿔서 코드 길이 비교
- ASP.NET Core에서 `IHttpClientFactory`로 HttpClient 관리하기
