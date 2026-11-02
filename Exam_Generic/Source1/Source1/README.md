# 제네릭 저장소: 제네릭 클래스·메서드와 제약 조건 연습

직원(`Employee`)과 장비(`Device`)처럼 서로 다른 타입을 **저장소 클래스 하나**로 저장하고, ID로 찾고, 삭제하는 콘솔 프로그램입니다.

Delphi에서는 `TList<T>`, `TDictionary<K,V>`처럼 이미 만들어진 제네릭을 쓰는 경우가 대부분이었습니다.
이번에는 제네릭 클래스와 메서드를 직접 만들어 보면서, 특히 **`where T : class` 같은 제약이 왜 필요한지**를 직접 확인하는 것이 목표였습니다.

## 구조

```
IEntity (string Id)
 ├─ record Employee(Id, Name, Department)
 └─ record Device(Id, Location, IsActive)

Repository<T> where T : class, IEntity
 ├─ Repository<Employee>
 └─ Repository<Device>

PrintList<T>(title, List<T>)                       제약 없음
PrintEntities<T>(title, IEnumerable<T>) where T : IEntity
```

## 진행 단계

| 단계 | 내용 |
|---|---|
| 1 | 제약 없는 제네릭 메서드 `PrintList<T>`: `string`, `int` 목록 모두 출력 |
| 2 | 제약 없는 제네릭 클래스 `Repository<T>`: `Add`, `GetAll`, `Count` |
| 3 | `GetById` 추가 → 제약 없이는 컴파일이 안 돼서 `where T : class, IEntity` 적용 |
| 4 | `Repository<Employee>`, `Repository<Device>`로 실제 사용, null 처리 |
| 5 | 내부를 `List<T>` → `Dictionary<string, T>`로 교체, 중복 ID 예외, `Remove` |
| 6 | 제약 있는 제네릭 메서드 `PrintEntities<T> where T : IEntity` |

## 실행 결과 (일부)

```
E002 조회: 마석도 (영업팀)
E999 조회: 없음
D03 조회: 창고 (중지)
직원 3명, 장비 3대
오류: 이미 존재하는 id입니다: E001
E003 삭제: True
E999 삭제: False
남은 직원 2명
[직원 목록]
[E001] Employee { Id = E001, Name = 홍길동, Department = 개발팀 }
[E002] Employee { Id = E002, Name = 마석도, Department = 영업팀 }
[장비 목록]
[D01] Device { Id = D01, Location = 정문, IsActive = True }
[D02] Device { Id = D02, Location = 후문, IsActive = True }
[D03] Device { Id = D03, Location = 창고, IsActive = False }
```

## 제약 조건이 필요한 이유 (이번 과제의 핵심)

3단계에서 `GetById`를 제약 없이 만들면 두 군데에서 컴파일 오류가 납니다.

```csharp
public T? GetById(string id)
{
    foreach (T item in items)
        if (item.Id == id)   // 오류 1: T에 Id가 있는지 모름
            return item;
    return null;             // 오류 2: T가 int일 수도 있어서 null 반환 불가
}
```

| 제약 | 해결하는 문제 |
|---|---|
| `IEntity` | T에 반드시 `Id`가 있음 → `item.Id` 사용 가능 |
| `class` | T는 참조 타입 → `return null` 가능, `T?`가 확실히 "null 가능"이라는 뜻이 됨 |

제약을 하나씩 지워 보고 어떤 오류가 나는지 직접 확인했습니다.
결국 **제약은 T로 할 수 있는 일을 늘려 주는 대신, 들어올 수 있는 타입을 줄인다**는 걸 알게 됐습니다.

그래서 6단계의 `PrintEntities`는 `IEntity`만 걸고 `class`는 걸지 않았습니다.
`Id`는 써야 하지만 `null`을 반환하지 않기 때문입니다. **메서드 안에서 실제로 쓰는 기능만큼만 제약을 건다**를 기준으로 삼았습니다.

| | `PrintList<T>` | `PrintEntities<T>` |
|---|---|---|
| 제약 | 없음 | `IEntity` |
| 받을 수 있는 타입 | 아무 타입 | `IEntity` 구현 타입만 |
| 메서드 안에서 쓸 수 있는 것 | `ToString()` | `ToString()` + `Id` |

매개변수 타입도 같은 원리입니다. `PrintList`는 `items[i]`, `Count`를 써야 해서 `List<T>`가 필요하지만,
`PrintEntities`는 `foreach`만 쓰므로 더 넓은 `IEnumerable<T>`로 받았습니다. 그러면 List, 배열, HashSet 모두 넘길 수 있습니다.

## Delphi와 비교

| Delphi | C# |
|---|---|
| `TRepository<T> = class` | `class Repository<T>` |
| `TRepository<T: class, IEntity>` | `class Repository<T> where T : class, IEntity` |
| `<T: constructor>` | `where T : new()` |
| `Result := nil` (class 제약 시) | `return null` |
| `if Assigned(Emp) then ...` | `emp?.Name ?? "없음"` |
| `TEnumerable<T>` | `IEnumerable<T>` |
| 클래스 끝 `end;` | 클래스 끝 `}` (세미콜론 없음) |

## 작성하면서 실수한 것들

**1. `Count` 프로퍼티가 항상 0**

`public int _count { get; }`로 선언하고 값을 넣지 않아서 항목을 추가해도 0이었습니다.
개수를 따로 저장하지 않고 `public int Count => items.Count;`로 읽을 때마다 계산하게 바꿨습니다.

**2. "코드 경로 중 일부만 값을 반환합니다" (CS0161)**

`GetById`에서 찾았을 때만 `return`하고, 끝까지 못 찾은 경우의 `return`을 빠뜨렸습니다.
`return null;`을 추가했는데, 이게 가능하려면 `class` 제약이 필요하다는 데까지 이어졌습니다.

**3. null 확인 없이 `.Name` 접근**

`GetById` 결과를 바로 `.Name`으로 읽어서, 없는 ID를 넣으면 `NullReferenceException`이 날 수 있었습니다.
반환 타입을 `T?`로 바꾼 덕분에 컴파일러가 경고(CS8602)로 미리 알려 줬고, `if`나 `?.` / `??`로 처리했습니다.

**4. `Remove`를 `try/catch`로 판단**

`Dictionary.Remove`가 실패하면 예외가 날 거라고 생각했는데, 실제로는 `false`를 반환합니다.
그래서 없는 ID를 지워도 항상 `true`가 나왔습니다. `return items.Remove(id);`로 고쳤습니다.
반환값으로 결과를 알려 주는 메서드는 예외가 아니라 반환값을 확인해야 합니다.

**5. `GetAll`이 내부 Dictionary를 그대로 반환**

내부를 Dictionary로 바꾸면서 `GetAll`의 반환 타입까지 바뀌었습니다.
내부 구조가 밖으로 드러나고, 밖에서 `Add`를 거치지 않고 데이터를 바꿀 수도 있었습니다.
`new List<T>(items.Values)`로 복사본을 반환하게 했습니다.

## 내부 구현을 바꿔도 사용하는 쪽은 그대로

5단계에서 저장소 내부를 `List<T>`에서 `Dictionary<string, T>`로 바꿨지만,
`empRepo.Add(...)`, `GetById(...)`를 사용하는 코드는 한 줄도 고치지 않았는데 같은 결과가 나왔습니다.
클래스로 감싸서 내부를 숨기는 이유를 이번에 직접 체감했습니다.

## 다음에 해 볼 것

- `Find(Func<T, bool> condition)`로 조건 검색 ("개발팀 직원", "사용중인 장비"): 2주차 람다와 연결
- `IRepository<T>` 인터페이스를 만들고 `Repository<T>`가 구현하게 하기
- 이름 관례 정리: private 필드는 `_items`, `readonly` 적용
