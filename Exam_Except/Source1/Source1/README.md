# 출입 로그 리포트: 예외 처리와 using 연습

앞서 만든 출입 기록 분석기를 확장해서, 로그를 읽고 분석 결과를 `report.txt`로 저장하는 프로그램입니다.
파일이 없거나, 줄 형식이 틀리거나, 저장에 실패하는 상황을 하나씩 처리하면서
C#의 `try/catch/finally`와 `using`을 익히는 것이 목적이었습니다.

Delphi에서는 `try..except`, `try..finally Free`를 습관처럼 썼는데,
C#에서는 같은 일을 어떻게 하는지, 그리고 GC가 있는데도 왜 정리 코드가 필요한지를 직접 확인해 보고 싶었습니다.

## 진행 단계

| 단계 | 내용 |
|---|---|
| 1 | 로그 파일·폴더가 없을 때 프로그램이 죽지 않고 종료 |
| 2 | 줄 해석을 `ParseLine` 메서드로 분리하고, 형식 오류는 예외로 던짐 |
| 3 | 줄 번호와 원문을 담는 사용자 정의 예외 `InvalidLogLineException` |
| 4 | `StreamWriter` + `using`으로 리포트 파일 저장 |
| 5 | `finally`로 성공·실패와 관계없이 소요 시간 출력 |
| 6 | `IDisposable`을 직접 구현한 `OperationTimer`로 5단계 대체 |

## 실행 결과

```
[시작] 리포트 생성
정상 9건, 오류 1건
  [10번째 줄] 형식 개수 오류 => 원문 : 잘못된 줄
[종료] 3
```

리포트 저장 경로를 없는 폴더로 바꾸면 `리포트를 저장할 폴더가 없습니다.`가 출력되고,
그 경우에도 `[종료]`는 출력됩니다.

## Delphi와 비교

| Delphi | C# |
|---|---|
| `try..except on E: EFOpenError do` | `catch (FileNotFoundException)`, `catch (DirectoryNotFoundException)` |
| 파일 없음·폴더 없음이 모두 `EFOpenError` | 예외 타입으로 구분됨 |
| `raise EConvertError.Create(...)` | `throw new InvalidLogLineException(...)` |
| `EMyError = class(Exception)` + 필드 | `class InvalidLogLineException : Exception` + 프로퍼티 |
| `try..finally Writer.Free` | `using var writer = new StreamWriter(...)` |
| `destructor Destroy` | `Dispose()` |
| `Exit` | `return` (최상위 코드에서는 프로그램 종료) |

## 작성하면서 실수한 것들

**1. `try` 안에서 선언한 변수를 밖에서 쓰려고 함**

`string[] text = await File.ReadAllLinesAsync(...)`를 `try` 안에서 선언했더니 `try` 밖에서 `text`를 쓸 수 없었습니다.
C# 변수는 선언된 `{ }` 블록 안에서만 살아 있어서, `try` 밖에 먼저 선언하는 것으로 고쳤습니다.
Delphi는 `var` 섹션에 미리 선언하니 겪어 보지 못한 문제였습니다.

`catch`에서 `return`을 빼먹으면 "할당되지 않은 변수 사용" 컴파일 오류가 나는데,
이게 "파일을 못 읽었으면 종료하라"는 힌트가 됐습니다.

**2. 줄 하나의 오류로 프로그램 전체가 종료됨**

줄 해석 코드를 파일 읽기용 `try` 안에 같이 넣었더니,
10번째 줄의 형식 오류가 파일 읽기의 `catch (Exception)`에 잡혀서 프로그램이 끝나 버렸습니다.

"파일을 못 읽음"은 멈춰야 하는 오류이고 "줄 하나가 틀림"은 기록하고 넘어갈 오류라서,
`try`를 분리하고 줄 처리는 **줄마다** `try/catch`로 감쌌습니다.
`try`를 어디에 두느냐가 "어디까지 실패로 볼 것인가"를 정한다는 걸 이번에 가장 크게 배웠습니다.

**3. `throw new Exception(...)`**

처음엔 일반 `Exception`을 던졌는데, 그러면 받는 쪽도 `catch (Exception)`으로 받아야 하고
코드의 진짜 버그까지 "오류 줄"로 처리되어 숨겨집니다.
형식 오류만 잡을 수 있도록 사용자 정의 예외로 바꿨습니다.

**4. `ParseLine`이 결과를 버림**

검사만 하는 `void` 메서드로 만들어서 파싱 결과가 사라졌습니다. `AccessLog`를 반환하도록 바꿨습니다.

**5. `finally`에서 스톱워치를 시작함**

`finally`에서 `Stopwatch.StartNew()`를 호출해서, 처리 시간이 아니라 방금 켠 스톱워치를 출력하고 있었습니다.
시작은 처리 전에, 정지와 출력은 `finally`에서 하도록 고쳤습니다.

**6. `StreamWriter`를 `try` 밖에서 생성**

저장 경로가 잘못되면 `try`에 들어가기도 전에 예외가 나서 `finally`가 실행되지 않았습니다.
생성 코드를 `try` 안으로 옮겼습니다.

**7. `catch` 없이 `try/finally`만 둠**

`finally`는 실행되지만 예외는 그대로 위로 올라가서 프로그램이 비정상 종료됐습니다.
`finally`는 정리를 보장할 뿐 예외를 처리하지는 않습니다. 저장 실패 메시지를 보여 주려고 `catch`를 추가했습니다.
Delphi의 `try..finally`와 같은 동작인데, C#에서 다시 확인하니 확실히 정리가 됐습니다.

## 예외를 쓸 때 기준으로 삼은 것

- **`TryParse` vs 예외**: 자주 일어나는 실패(사용자 입력, 로그의 깨진 줄)는 `TryXxx`처럼 `bool`로 받고,
  일어나면 안 되는 실패는 예외로 던진다. 예외는 무시하기 어렵지만 비용이 크다.
  이번 과제는 연습을 위해 줄 오류를 예외로 처리했지만, 오류 줄이 아주 많은 로그라면 `TryParseLine` 방식이 낫다.
- **구체적인 예외부터 잡는다**: `DirectoryNotFoundException`은 `IOException`의 자식이라 먼저 써야 한다. 순서가 반대면 컴파일 오류.
- **`catch (Exception)`은 가장 바깥에서만**: 안쪽에서 쓰면 진짜 버그가 묻힌다. 비워 두는 `catch { }`는 금지.
- **다시 던질 때는 `throw;`**: `throw ex;`는 예외 발생 위치가 바뀌어서 원인 추적이 어려워진다. Delphi의 `raise;`에 해당.

## IDisposable 정리

### 목적

GC는 **메모리만**, 그리고 **정해지지 않은 시점에** 정리합니다.
파일, DB 연결, 네트워크 소켓 같은 외부 자원은 다 쓴 즉시 닫아야 하는데 GC를 기다릴 수 없습니다.
그래서 "다 쓰면 정리가 필요한 객체"는 `IDisposable`을 구현해 `Dispose()`에 정리 코드를 두고,
쓰는 쪽은 `using`으로 예외가 나도 `Dispose()`가 반드시 호출되게 합니다.

```csharp
using var writer = new StreamWriter("report.txt");
```

는 컴파일러가 아래처럼 바꿔 줍니다. Delphi의 `try..finally Free`와 같은 구조입니다.

```csharp
var writer = new StreamWriter("report.txt");
try { ... }
finally { writer.Dispose(); }
```

`StreamWriter`를 `using` 없이 쓰면 버퍼가 비워지기 전에 프로그램이 끝나서
파일 끝부분이 저장되지 않을 수 있습니다.

### using 두 가지 형태

| 형태 | Dispose 시점 |
|---|---|
| `using (var x = ...) { }` | 중괄호 블록이 끝날 때 |
| `using var x = ...;` | 변수가 선언된 블록이 끝날 때 (C# 8~) |

파일 맨 위의 `using System.Diagnostics;`는 Delphi의 `uses`이고 자원 정리와는 관계가 없습니다.
같은 단어가 세 가지 뜻으로 쓰여서 처음에 헷갈렸습니다.

### 자주 만나는 IDisposable

| 분야 | 예 |
|---|---|
| 파일 | `StreamReader`, `StreamWriter`, `FileStream` |
| DB | `SqlConnection`, `SqlCommand`, EF Core `DbContext` |
| 네트워크 | `TcpClient`, `NetworkStream` |
| 동시성 | `SemaphoreSlim`, `CancellationTokenSource` |
| WinForms 그래픽 | `Graphics`, `Bitmap`, `Font` |

UniDAC의 `TUniConnection`, `TUniQuery`를 `try..finally Free`로 감싸던 자리에 C#에서는 `using`이 들어갑니다.

### 직접 구현하는 경우

**1. 내 클래스가 IDisposable 객체를 필드로 가지고 있을 때 (본래 용도)**

```csharp
class ReportWriter : IDisposable
{
    private readonly StreamWriter _writer;
    public ReportWriter(string path) => _writer = new StreamWriter(path);
    public void Dispose() => _writer.Dispose();   // 내가 가진 자원을 대신 정리
}
```

Delphi에서 필드로 `Create`한 객체를 `destructor Destroy`에서 `Free`해 주던 것과 같은 책임입니다.

**2. 범위가 끝날 때 반드시 실행할 마무리 동작 (응용)**

이번에 만든 `OperationTimer`가 이 경우입니다.
스톱워치는 해제할 자원이 아니기 때문에 `IDisposable`의 본래 목적과는 다르고,
"블록이 끝나면 예외가 나도 `Dispose()`가 호출된다"는 동작을 눈으로 확인하려고 만들었습니다.
`ILogger.BeginScope`, `TransactionScope`처럼 .NET에도 이런 패턴이 있지만,
자원이 없는데 `IDisposable`을 쓰는 건 남용이라는 의견도 있어서 응용으로만 알아 두기로 했습니다.

### 알아 둘 것

- `Dispose()`는 여러 번 호출되어도 문제가 없게 만드는 게 관례
- 비동기로 정리해야 하는 자원은 `IAsyncDisposable`과 `await using`을 쓴다
- 면접 답변용 한 줄: *GC는 메모리만, 정해지지 않은 시점에 정리하므로, 파일·DB 연결 같은 자원은 `IDisposable`로 정리 시점을 명시하고 `using`으로 예외가 나도 정리되게 한다.*

## 다음에 고칠 것

- 1단계 `catch`별 한국어 메시지: 지금은 세 `catch`가 모두 `ex.Message`를 출력해서 타입별로 나눈 의미가 약함
- `report.txt` 첫 줄에 생성 시각, 정상/오류 건수 추가
- `[종료]` 출력에 작업 이름과 `ms` 단위 추가
- 오타 정리: `LineNumer`, `ParseLIne`
- `OperationTimer` 대신 `ReportWriter`처럼 실제 자원을 가진 클래스로 `IDisposable` 구현해 보기
