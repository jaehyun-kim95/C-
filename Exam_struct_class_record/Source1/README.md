# struct / class / record 복사 동작과 박싱·언박싱

같은 드론 위치 좌표(`X`, `Y`)를 `struct`, `class`, `record` 세 가지로 만들어 두고,
대입·메서드 전달·비교·컬렉션·박싱에서 각각 어떻게 다르게 동작하는지 실험한 프로젝트입니다.

각 실험은 결과를 실행 전에 먼저 예측해서 코드에 적고,
`Check("설명", 조건)` 함수로 **통과/실패**를 출력하는 작은 테스트 형태로 작성했습니다.

```csharp
void Check(string label, bool ok) => Console.WriteLine($"{(ok ? "통과" : "실패")} : {label}");
```

## 시작 전에 정리한 것: C# record ≠ Delphi record

| Delphi | C# | 종류 |
|---|---|---|
| `record` | **`struct`** | 값 타입 |
| `class` | `class` | 참조 타입 |
| (없음) | `record` | 참조 타입 + 값 비교 |

이름이 같아서 처음에는 C# `record`가 Delphi `record`와 같은 값 타입이라고 생각하기 쉬운데,
Delphi `record`에 해당하는 것은 C# `struct`이고, C# `record`는 class의 한 종류입니다.

## 실험 대상

```csharp
struct PositionS : IMovable { public int X { get; set; } public int Y { get; set; } ... }
class  PositionC : IMovable { public int X { get; set; } public int Y { get; set; } ... }
record PositionR(int X, int Y);
```

`IMovable`은 `Move()`(X를 10 증가) 하나만 가진 인터페이스로, 박싱 실험에 사용했습니다.

## 1. 대입

```csharp
var s2 = s1;  s2.X = 99;    // s1.X == 1   (struct: 값 전체 복사)
var c2 = c1;  c2.X = 99;    // c1.X == 99  (class: 참조 복사, 같은 객체)
var r3 = r1 with { X = 99 };// r1.X == 1   (record: 값 변경 불가, with로 새 객체)
```

record는 참조 타입이라 `r2 = r1`이면 `ReferenceEquals(r1, r2)`가 `true`입니다.
대신 값을 바꿀 수 없어서 공유해도 안전하고, 바꾸고 싶으면 `with`로 새 객체를 만듭니다.

## 2. 메서드에 넘길 때

| 메서드 | 결과 | 이유 |
|---|---|---|
| `MoveS(PositionS p)` | 원본 그대로 | 복사본이 넘어감 |
| `MoveRefS(ref PositionS p)` | 원본 변경 | 원본 자체가 넘어감 |
| `MoveC(PositionC p)` | 원본 변경 | 같은 객체를 가리키는 참조가 넘어감 |
| `ReplaceC(PositionC p) { p = new ...; }` | **원본 그대로** | 지역 변수 `p`만 다른 객체를 가리키게 됨 |

가장 중요하게 정리한 부분은 마지막 줄입니다.
class를 넘기는 것은 "객체를 넘기는 것"이 아니라 **"참조를 복사해서 넘기는 것"**이라,
메서드 안에서 새 객체를 대입해도 호출한 쪽 변수는 바뀌지 않습니다.

이 규칙은 Delphi와 같습니다. record를 값으로 넘기면 복사본, `var`를 붙이면 원본이 넘어가고,
class 매개변수에 프로시저 안에서 `Create`로 새 객체를 대입해도 호출한 쪽은 그대로입니다.

## 3. 비교

| | `Equals` (값이 같은 두 객체) | `==` |
|---|---|---|
| struct | `true` | 컴파일 오류 (직접 정의해야 함) |
| class | `false` | `false` |
| record | `true` | `true` |

class는 값이 같아도 다른 객체면 다르다고 판단합니다.
record는 컴파일러가 값 비교용 `Equals`와 `==`를 자동으로 만들어 줍니다.
데이터를 담는 용도에 record를 쓰는 이유 중 하나입니다.

## 4. 컬렉션 안의 struct

```csharp
listS[0].X = 5;              // 컴파일 오류 CS1612
var tmp = listS[0];          // 꺼내서
tmp.X = 5;                   // 바꾸고
listS[0] = tmp;              // 다시 넣어야 함

arrS[0].X = 5;               // 배열은 가능
listC[0].X = 5;              // class 목록은 가능
foreach (var p in listS) p.X = 0;   // 컴파일 오류 CS1654
```

`List<T>`의 인덱서는 요소를 **복사해서** 돌려주기 때문에, struct는 직접 바꿀 수 없습니다.
그래서 struct는 "작고, 한 번 만들면 바꾸지 않는 값"으로 쓰는 것이 관례이고,
`readonly struct`로 선언하면 아예 값 변경을 막을 수 있습니다. `DateTime`, `decimal`이 이런 struct입니다.

## 5. 박싱과 언박싱

- **박싱**: 값 타입을 `object`나 인터페이스로 다룰 때 값을 힙의 새 상자에 **복사**하는 것
- **언박싱**: 상자에서 값을 꺼내 다시 값 타입으로 바꾸는 것

```csharp
int n = 10;
object o = n;     // 박싱 (복사)
n = 20;           // (int)o는 여전히 10

long l = (long)o;         // InvalidCastException: 넣은 타입(int)으로만 꺼낼 수 있음
long ok = (long)(int)o;   // int로 꺼낸 뒤 변환하면 됨
```

가장 조심해야 할 부분은 **struct를 인터페이스로 다룰 때**입니다.

```csharp
var sm = new PositionS(1, 1);
IMovable m = sm;    // 여기서 박싱
m.Move();           // 상자 안의 복사본이 움직임
// sm.X == 1, ((PositionS)m).X == 11
```

오류도 경고도 없이 원본이 아니라 복사본이 바뀝니다.
같은 코드를 class로 하면 박싱이 없으므로 원본이 바뀝니다.

## 6. 박싱 비용 측정

정수 100만 개를 `ArrayList`(object 목록)와 `List<int>`에 넣고 합계를 구해 시간과 할당 메모리를 비교했습니다.

| | 동작 | 결과 |
|---|---|---|
| `ArrayList` | 넣을 때마다 박싱, 꺼낼 때마다 언박싱 | 할당 메모리가 몇 배 많고 더 느림 |
| `List<int>` | 박싱 없음 | 빠르고 할당이 적음 |

측정값은 PC마다 다르며, `GC.GetAllocatedBytesForCurrentThread()`와 `Stopwatch`로 확인했습니다.
제네릭 저장소 과제에서는 제네릭의 **타입 안전성**을 확인했다면, 이번에는 **박싱이 없다**는 두 번째 이점을 직접 확인했습니다.

### 박싱이 일어나는 곳

- 값 타입을 `object` 변수나 매개변수에 넣을 때 (`void Log(object value)`)
- 제네릭이 아닌 옛 컬렉션 (`ArrayList`, `Hashtable`)
- struct를 인터페이스 타입으로 다룰 때

## 최종 정리

| | struct | class | record |
|---|---|---|---|
| 종류 | 값 타입 | 참조 타입 | 참조 타입 |
| 대입 | 값 복사 | 참조 복사 | 참조 복사 |
| 메서드 전달 | 복사본 | 참조의 복사 | 참조의 복사 |
| `==` | 기본 없음 | 같은 객체인지 | 값 비교 |
| 값 변경 | 가능 (비권장) | 가능 | 불가 (`with`) |
| 쓰는 곳 | 좌표, 금액 같은 작은 값 | 상태가 바뀌는 객체 | DTO, 조회 결과 |
| Delphi 대응 | `record` | `class` | 없음 |

값 타입이면서 값 비교도 필요하면 `record struct`도 있습니다.

## 다음에 해 볼 것

- `readonly struct`로 바꿔서 4단계의 수정 코드가 모두 컴파일 오류가 되는지 확인
- `record struct`로 같은 실험을 해서 struct·record와 비교
- 6단계를 xUnit 테스트와 BenchmarkDotNet으로 다시 측정
