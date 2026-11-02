# Delphi 클래스 C# 이식 연습: 직원 급여 계산

Delphi로 6년 동안 개발하다가 C#으로 넘어가면서 처음 해 본 이식 과제입니다.
Delphi로 작성된 직원 급여 계산 유닛을 C# 콘솔 앱으로 옮기면서
두 언어의 클래스 문법이 어떻게 다른지 직접 확인하는 것이 목적이었습니다.

## 무엇을 만들었나

정규직과 시간제 직원의 급여를 계산하는 작은 콘솔 프로그램입니다.

```
IPayable (급여 계산 인터페이스)
 └─ TEmployee (추상 클래스: ID, 이름, 입사일, 근속 연수)
     ├─ TFullTimeEmployee  정규직: 월급 + 근속 1년당 50,000원
     └─ TPartTimeEmployee  시간제: 시급 × 누적 근무시간
```

- 직원을 `List<TEmployee>` 하나에 담아 급여 정보를 출력하고 총 지급액을 합산
- 이름이 비어 있거나, 월급이 음수이거나, 근무시간이 0 이하이면 `ArgumentException` 발생

## 실행

```bash
dotnet run
```

```
[ID : 1] 홍길동, 고용일자 : 2020-08-01 (근속 6년) 월급 : 350000
[ID : 2] 마석도, 고용일자 : 2019-08-01 (근속 7년) 시급 : 12000
지불할 금액 : 698,000
이름은 비어 있을 수가 없습니다.
```

근속 연수는 실행 날짜 기준으로 계산되므로 날짜에 따라 금액이 달라집니다. (위 결과는 2026-10-14 기준)

## Delphi와 비교하며 정리한 것

| Delphi | C# | 메모 |
|---|---|---|
| `IPayable = interface` + GUID | `interface IPayable` | GUID 불필요, `TInterfacedObject` 상속도 필요 없음 |
| `virtual; abstract;` | `abstract class`, `abstract` 메서드 | 추상 클래스는 `new`로 직접 생성하면 컴파일 오류 |
| `inherited Create(...)` | 생성자 뒤 `: base(...)` | |
| `inherited GetPayInfo` | `base.GetPayInfo()` | |
| `property Name read FName write SetName` | `get` / `set` 프로퍼티 | setter 안에 검증 로직을 넣는 구조는 같음 |
| `Currency` | `decimal` | |
| `raise EArgumentException.Create(...)` | `throw new ArgumentException(...)` | |
| `TObjectList<T>` + `Free` | `List<T>` | 해제 코드가 없어도 GC가 처리 |

## 작성하면서 실수한 것들

실제로 막혔던 부분이라 따로 적어 둡니다.

**1. `return` 누락 (CS0161)**
`override` 메서드에서 `base.GetPayInfo();`만 호출하고 반환을 안 해서 계속 빨간 줄이 떴습니다.
Delphi는 `Result`를 안 넣어도 경고만 나오는데 C#은 컴파일 오류입니다.
`=>` 식 본문으로 바꾸면 오류가 사라졌는데, 식 본문은 그 식 자체가 반환값이 되기 때문이었습니다.

**2. 날짜 형식 `mm`과 `MM`**
`ToString("yyyy-mm-dd")`로 썼더니 월 자리에 `00`이 나왔습니다. C#에서 `mm`은 분이고 월은 `MM`입니다.

**3. 생성자가 검증을 건너뜀**
생성자에서 `_name = name;`처럼 필드에 바로 넣어서, 빈 이름으로도 객체가 만들어졌습니다.
`Name = name;`으로 프로퍼티를 거치게 바꿔서 해결했습니다.
Delphi 원본에서도 `FName := AName`이 아니라 `Name := AName`으로 썼던 이유가 이것이었습니다.

**4. 근속 연수 계산**
처음에는 `오늘 연도 - 입사 연도`로 계산했는데, 입사 기념일이 아직 안 지난 경우 1년이 더 붙었습니다.
근속 수당이 급여에 들어가기 때문에 금액이 틀리게 나오는 버그였습니다.

```csharp
int years = today.Year - _hire.Year;
if (_hire.Date > today.AddYears(-years)) years--;
```

**5. 금액 타입**
처음엔 `int`로 하다가 소수 시간(25.5시간)을 넣으려고 `double`로 바꿨는데,
`double`은 부동소수점 오차가 있어서 금액 계산에는 맞지 않다는 걸 알고 `decimal`로 통일했습니다.

**6. 테스트 코드 구조**
정상 케이스와 오류 케이스를 하나의 `try` 블록에 넣었더니,
첫 줄에서 예외가 나면서 나머지 코드가 전부 실행되지 않았습니다.
오류 케이스마다 `try/catch`를 따로 두는 쪽으로 바꿨습니다.

## 프로퍼티와 메서드

근속 연수를 프로퍼티로 할지 메서드로 할지 고민했습니다.
값이 가볍게 계산되는 "속성"이라 프로퍼티가 어울리지만,
실행 날짜에 따라 값이 바뀌기 때문에 메서드로 두는 것도 근거가 있다고 판단해 `YearsOfService()` 메서드로 두었습니다.

## 다음에 고칠 것

- 이름 관례: Delphi 습관대로 `T` 접두어, `F`/`A` 접두어를 썼는데 C#에서는 쓰지 않는다는 걸 알게 됐습니다.
  (`TEmployee` → `Employee`, `FId` → `Id`, 매개변수는 소문자 시작) 다음 과제부터 C# 관례로 작성할 예정입니다.
- 생성자 오류 케이스(빈 이름, 음수 월급) 테스트 추가
- 출력 금액에 천 단위 구분(`N0`) 적용
