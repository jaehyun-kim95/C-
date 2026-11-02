# 출입 기록 분석기: List · Dictionary · HashSet 연습

Delphi에서 `TStringList`, `TDictionary`로 자주 하던 "텍스트 로그 읽어서 집계하기"를
C#의 `List`, `Dictionary`, `HashSet`으로 다시 짜 본 연습입니다.

같은 일을 하는 Delphi 코드를 먼저 두고, C#으로 옮기면서
각 컬렉션을 **언제 쓰는지**를 기준으로 정리하는 게 목표였습니다.
LINQ는 일부러 쓰지 않고 `foreach`와 컬렉션 메서드만으로 작성했습니다.

## 하는 일

사원증 출입 로그(`access.log`)를 읽어서 아래 내용을 출력합니다.

1. 정상 기록과 오류 줄 구분
2. 직원별 출입 횟수 (사번 순)
3. 출입구별 출입 횟수와 이용 직원 (중복 없이)
4. 이틀 모두 출근한 직원 / 하루만 출근한 직원

입력 형식은 `일시,사번,이름,출입구,구분` 입니다.

```
2026-10-12 08:51,E001,홍길동,정문,IN
2026-10-12 08:55,E002,마석도,후문,IN
...
잘못된 줄
```

## 실행 결과

```
정상적인 줄 : 9, 오류 줄 : 1
E001 홍길동 : 4
E002 마석도 : 3
E003 김민수 : 1
E004 이서연 : 1
정문 : 6 / 4명 E001,E002,E003,E004
후문 : 3 / 2명 E001,E002

이틀 모두 출근: 홍길동, 마석도
10-12에만 출근: 김민수
10-13에만 출근: 이서연
```

## 컬렉션을 고른 기준

| 쓰인 곳 | 컬렉션 | 이유 |
|---|---|---|
| 정상 기록, 오류 줄 | `List<AccessLog>`, `List<string>` | 개수를 모르고 하나씩 추가하면서 모음. 순서 유지 |
| 직원별 횟수, 사번 → 이름 | `Dictionary<string, int>`, `Dictionary<string, string>` | 사번을 키로 바로 찾고 누적 |
| 출입구별 이용 직원 | `Dictionary<string, HashSet<string>>` | 같은 사람이 여러 번 지나가도 한 번만 |
| 날짜별 출근자 비교 | `Dictionary<DateTime, HashSet<string>>` + 집합 연산 | 교집합·차집합을 메서드 하나로 |

정문은 6번 지나갔지만 사람은 4명입니다.
`int`로 세는 것과 `HashSet`으로 모으는 것의 차이가 이 숫자 차이로 보여서 이해하기 좋았습니다.

## Delphi 코드와 비교

| Delphi | C# |
|---|---|
| `TStringList.LoadFromFile` | `File.ReadAllLinesAsync` |
| `Delimiter` + `StrictDelimiter` + `DelimitedText` | `Split(',')` |
| `TryStrToDateTime` | `DateTime.TryParse(..., out var dt)` |
| 필드를 `Fields[1]`, `Fields[3]`처럼 번호로 접근 | `record AccessLog`로 만들어 `log.Id`, `log.Gate`로 접근 |
| `AddOrSetValue` | 인덱서 대입 `names[id] = name` |
| 키 정렬용 `TStringList` (`Sorted := True`) | `new List<string>(dict.Keys)` 후 `Sort()` |
| `TStringList` + `dupIgnore` | `HashSet<string>` |
| `TDictionary<string, Boolean>`로 집합 흉내 | `HashSet<string>` |
| 교집합을 루프 + `ContainsKey`로 직접 계산 | `IntersectWith`, `ExceptWith` |
| `TObjectDictionary` + `doOwnsValues`, `try..finally Free` | 해제 코드 없음 |
| `CommaText` | `string.Join(",", list)` |

Delphi 원본은 날짜를 `if Day = '2026-10-12'`처럼 코드에 직접 적었는데,
C#에서는 `Dictionary<DateTime, HashSet<string>>`로 날짜별로 자동으로 모으게 바꿨습니다.
날짜는 문자열을 자르지 않고 `log.Time.Date`로 꺼냈습니다.

## 작성하면서 실수한 것들

**1. HashSet 하나를 모든 출입구가 같이 씀**

루프 밖에서 `new HashSet<string>()`을 한 번만 만들고, 처음 보는 출입구마다 그 집합을 Dictionary에 넣었습니다.
결과는 후문도 4명으로 나왔습니다.

HashSet은 참조 타입이라 Dictionary에 넣을 때 복사되지 않고 **같은 객체**가 들어갑니다.
정문과 후문이 집합 하나를 공유하고 있었던 겁니다.
출입구마다 `new HashSet<string>()`을 새로 만들어 넣는 것으로 고쳤습니다.

반대로 `Dictionary<string, int>`의 값은 값 타입이라 꺼내서 바꿔도 반영이 안 되고,
`counts[id]++`처럼 Dictionary에 다시 써야 합니다. 이 차이를 여기서 처음 체감했습니다.

**2. 차집합을 엉뚱한 집합에 적용**

"하루만 출근" 집합을 복사해 놓고, `ExceptWith`는 복사 붙여넣기 때문에 교집합 변수(`both`)에 호출했습니다.
교집합은 비어 버리고, 하루만 출근 목록에는 그날 출근자 전원이 나왔습니다.

집합 연산 메서드는 새 결과를 돌려주지 않고 **호출한 집합 자체를 바꾸기** 때문에
엉뚱한 변수에 호출해도 컴파일 오류가 나지 않습니다. 결과를 출력해서 확인하는 게 중요하다는 걸 배웠습니다.
같은 이유로, 원본 집합(`day1`, `day2`)을 지키려고 연산 전에 `new HashSet<string>(day1)`로 복사했습니다.

## Dictionary 사용 시 기억할 것

| 코드 | 키가 없을 때 | 키가 있을 때 |
|---|---|---|
| `dict.Add(key, value)` | 추가 | 예외 |
| `dict[key] = value` | 추가 | 덮어씀 |
| `dict[key]` (읽기) | `KeyNotFoundException` | 값 반환 |

그리고 `Dictionary`, `HashSet` 모두 순서를 보장하지 않기 때문에, 출력 순서가 필요하면 따로 정렬해야 합니다.

## 다음에 해 볼 것

- 같은 기능을 LINQ(`GroupBy`, `Distinct`, `Intersect`)로 다시 작성해서 코드 길이 비교
- `GetValueOrDefault`, `TryGetValue`로 누적 코드 줄이기
- 설정 파일(`workStart=09:00`)을 Dictionary로 읽어서 지각자 찾기
- 파일 경로를 하드코딩하지 않고 실행 폴더 기준으로 읽기
