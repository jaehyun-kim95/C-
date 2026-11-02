# nullable reference types: 회의실 예약 조회 코드의 null 정리

null 처리가 전혀 안 된 회의실 예약 조회 코드를 받아서,
C#의 nullable reference types 기능을 켜고 경고를 하나씩 해결한 연습입니다.
새로 짜는 대신 **레거시 코드를 넘겨받아 고치는 상황**으로 진행했습니다.

Delphi에서는 객체 변수가 언제든 `nil`일 수 있어서 `Assigned()`로 직접 확인해 왔고,
문자열은 기본값이 빈 문자열이라 nil을 신경 쓸 일이 없었습니다.
C#은 `string`도 class라서 `null`이 될 수 있고, 그 대신 컴파일러가 위험한 곳을 경고해 준다는 점이 가장 큰 차이였습니다.

## 데이터

```csharp
class MeetingRoom { Id, Name, Capacity }
class Person      { Name, Email }
class Reservation { RoomId, Organizer, Memo }
```

예약 3건 중 하나는 이메일·메모가 없고, 하나는 존재하지 않는 회의실(R3)에 예약자도 없습니다.

## 1. 기능이 꺼진 상태

파일 맨 위에 `#nullable disable`을 두고 실행했습니다.
경고 없이 컴파일됐지만 두 번째 예약의 이메일(`null`)에서 `NullReferenceException`으로 프로그램이 종료됐습니다.
코드에는 위험한 곳이 더 있었지만, 실행해 보기 전에는 알 수 없었습니다.

## 2. 기능을 켜고 나온 경고

| 경고 | 의미 |
|---|---|
| CS8618 | null이 아니어야 하는 속성이 초기화되지 않을 수 있음 |
| CS8603 | null을 반환할 수 있음 (`return null`) |
| CS8625 | null 리터럴을 null 불가 타입에 대입 |

이 시점에는 컴파일러가 "모든 타입은 null이 아니다"라고 믿고 있어서,
정작 위험한 출력 줄에는 경고가 거의 나오지 않았습니다.

## 3. 업무 규칙으로 null 가능 여부 정하기

경고를 없애는 게 목적이 아니라, **각 값이 업무상 비어 있을 수 있는지**를 정하는 단계였습니다.

| 속성 | 업무 규칙 | 타입 |
|---|---|---|
| `MeetingRoom.Id`, `Name` | 반드시 있음 | `required string` |
| `Person.Name` | 반드시 있음 | `required string` |
| `Person.Email` | 없는 사람도 있음 | `string?` |
| `Reservation.RoomId` | 반드시 있음 | `required string` |
| `Reservation.Organizer` | 외부 손님은 미지정 | `Person?` |
| `Reservation.Memo` | 선택 | `string?` |
| `FindRoom` 반환값 | 못 찾을 수 있음 | `MeetingRoom?` |

필수 값에는 `required`를 붙였습니다. 객체를 만들 때 값을 빠뜨리면 컴파일 오류가 나서,
"필수"라는 업무 규칙을 컴파일러가 강제해 줍니다.

이렇게 정하고 나니 앞의 경고들은 사라지고, 대신 출력 줄 네 곳에 **CS8602**(null일 수 있는 값 역참조)가 새로 나타났습니다.
1단계에서 실행하다 죽은 이메일 줄이 그중 하나였습니다. **실행하지 않고도 위험한 곳 전부를 찾아낸 것**이 이 기능을 켜는 이유라는 걸 체감했습니다.

## 4. `?.`와 `??`로 출력 정리

```csharp
Console.WriteLine($"예약자: {r.Organizer?.Name ?? "(미지정)"}");
Console.WriteLine($"이메일 도메인: {r.Organizer?.Email?.Split('@')[1] ?? "(없음)"}");
Console.WriteLine($"메모 길이: {r.Memo?.Length ?? 0}");
```

- `?.`: 왼쪽이 null이면 뒤를 실행하지 않고 null을 반환
- `??`: 왼쪽이 null이면 오른쪽 값을 사용
- 이메일 줄은 예약자도, 이메일도 없을 수 있어서 `?.`가 두 번 필요

Delphi의 `if Assigned(R.Organizer) then S := R.Organizer.Name else S := '(미지정)';`를 한 줄로 쓴 형태입니다.

## 5. null이면 건너뛰기

존재하지 않는 회의실은 기본 문구로 보여 줄 대상이 아니라 처리하면 안 되는 데이터라고 보고, 건너뛰도록 바꿨습니다.

```csharp
MeetingRoom? room = FindRoom(r.RoomId);
if (room is null)
{
    Console.WriteLine($"[건너뜀] 존재하지 않는 회의실: {r.RoomId}");
    continue;
}
Console.WriteLine($"회의실: {room.Name} (정원 {room.Capacity}명)");   // ?. 없이도 경고 없음
```

`if` 아래에서는 `room.Name`을 바로 써도 경고가 나오지 않습니다.
컴파일러가 "null이면 이미 `continue`로 빠졌다"는 흐름을 분석하기 때문이고, `continue`를 지우면 경고가 다시 생깁니다.

| null의 의미 | 처리 |
|---|---|
| 없으면 기본값으로 보여 주면 됨 (이메일, 메모) | `?.` + `??` |
| 없으면 작업을 할 수 없음 (회의실) | `is null` 검사 후 `continue`/`return` |

## 6. 메서드 매개변수와 CS8604

`SendReminder(Person organizer, ...)`에 `Person?`인 `r.Organizer`를 넘기면 CS8604 경고가 납니다.
호출 전에 `if (r.Organizer is not null)`로 확인해서 해결했습니다.

메서드 안에는 `ArgumentNullException.ThrowIfNull(organizer);`를 두었습니다.
nullable 경고는 **컴파일할 때의 검사일 뿐** 실행 중에 null을 막아 주지는 않기 때문에,
공용 메서드는 안에서도 한 번 더 방어하는 것이 관례입니다.

## 7. `??=`와 `!`

**`??=`**: `a = a ?? b`의 축약형으로, 왼쪽이 null일 때만 대입합니다.

```csharp
r.Memo ??= "(메모 없음)";
```

`??`는 값을 읽기만 하고 원본은 null로 남지만, `??=`는 원본을 실제로 바꿉니다.
그래서 이 코드를 실행한 뒤에는 메모 길이 출력도 바뀌었습니다.
Delphi의 `if not Assigned(FList) then FList := TStringList.Create;` 같은 지연 생성 패턴에 자주 씁니다.

**`!`**: null 경고를 끄는 연산자입니다.

```csharp
var r9 = FindRoom("R9")!;   // 경고 없음
Console.WriteLine(r9.Name); // 실행 중 NullReferenceException
```

값을 전혀 확인하지 않고 경고만 없애는 것이라 1단계의 꺼진 상태와 다를 게 없습니다.
최종 코드에서는 `!`를 쓰지 않았습니다.

## 작성하면서 실수한 것들

**1. 업무 규칙 표에 `?`를 빠뜨림**

"없는 사람도 있음", "선택"이라고 판단은 했는데 타입은 `string` 그대로 적었습니다.
`?`가 바로 "없을 수 있다"는 표시라는 걸 놓친 것입니다.
`Organizer`는 이름이 아니라 예약자 객체라서 타입이 `string`이 아니라 `Person?`이어야 했습니다.

**2. 반환 타입을 안 바꿨더니 위험한 줄에 경고가 없었음**

클래스 속성은 고쳤지만 `FindRoom`의 반환 타입은 `MeetingRoom` 그대로 두었습니다.
그러자 컴파일러가 "null을 반환하지 않는다"는 선언을 믿고 `room.Name` 줄에 경고를 띄우지 않았습니다.
**`?`를 정확히 붙여야 경고도 정확해진다**는 걸 이때 알았습니다.

**3. `?.`만 쓰고 `??`를 빠뜨림**

`r.Memo?.Length`처럼 `?.`만 쓰니 경고는 0개가 됐지만, null인 곳은 빈칸으로 출력됐습니다.
`?.`는 프로그램이 죽지 않게 해 줄 뿐 결과는 여전히 null이고, 문자열 보간은 null을 빈 문자열로 출력합니다.
**경고 0개는 "null 때문에 죽지 않는다"는 뜻이지 "원하는 결과가 나온다"는 뜻이 아니어서,** 기대 출력과 비교하는 확인이 따로 필요했습니다.

**4. null 검사를 엉뚱한 곳에 넣음**

5단계에서 `FindRoom` 안의 회의실 목록 항목에 `is null` 검사를 넣었습니다.
그런데 목록의 항목은 null이 아니고, null이 생기는 곳은 못 찾았을 때의 반환값입니다.
**null 검사는 null을 만드는 곳이 아니라 받아서 쓰는 곳에서 한다**로 정리했습니다.

**5. 문자열 보간 안의 괄호 누락**

`$"(알 수 없음: {r.RoomId})"`처럼 보간 `{}` 뒤에 출력용 `)`가 필요한 곳에서 두 번이나 괄호를 빠뜨렸습니다.
컴파일 오류가 아니라 출력만 이상해지는 실수라 출력을 직접 확인해야 잡힙니다.

## 정리

| 상황 | 방법 |
|---|---|
| 반드시 있어야 하는 값 | `?` 없이 + `required` 또는 생성자 |
| 없을 수 있는 값 | `?` |
| 없으면 기본값 표시 | `?.` + `??` |
| 없으면 처리 불가 | `is null` 검사 후 빠져나가기 |
| null이면 기본값을 채워 두기 | `??=` |
| 공용 메서드 매개변수 | `ArgumentNullException.ThrowIfNull` |
| 경고 끄기 | `!` (확실한 근거가 있을 때만) |

`int?`는 실제로 다른 타입(`Nullable<int>`)이지만 `string?`은 컴파일할 때만 의미가 있는 표시라는 점도 함께 정리했습니다.

## 다음에 해 볼 것

- `.csproj`에 `<WarningsAsErrors>Nullable</WarningsAsErrors>`를 넣어 null 경고를 오류로 만들기
- 앞서 만든 과제들(제네릭 저장소, 예외 처리)에서 nullable 경고가 남아 있는지 다시 확인
