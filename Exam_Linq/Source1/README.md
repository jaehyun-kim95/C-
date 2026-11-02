# LINQ 20문제: 온라인 서점 주문 데이터

책·고객·주문 세 개의 목록을 두고 LINQ 문제 20개를 풀었습니다.
`Where`, `Select`, `OrderBy`, `Any`/`All`, `Sum`/`Average`/`Aggregate`, `GroupBy`, `Join`을 순서대로 다루고,
일부 문제는 `foreach`로 먼저 풀어 본 다음 같은 결과를 LINQ로 다시 작성했습니다.

Delphi로 일하면서 데이터 조회는 거의 SQL로 해 왔기 때문에,
**SQL로 하던 일을 C# 코드 안에서 어떻게 하는지**를 익히는 데 초점을 맞췄습니다.

## 데이터

```csharp
record Book(int Id, string Title, string Author, string Category, decimal Price, int Stock);
record Customer(int Id, string Name, string City, bool IsMember);
record Order(int Id, int CustomerId, int BookId, int Quantity, DateTime OrderDate);
```

책 8권, 고객 5명, 주문 8건입니다. `Order`의 `CustomerId`, `BookId`가 DB의 외래 키 역할을 합니다.

## SQL과 LINQ 대응

| SQL | LINQ | 문제 |
|---|---|---|
| `WHERE` | `Where` | 1~3 |
| `SELECT` | `Select` (익명 타입 `new { ... }`) | 4~6 |
| `ORDER BY`, `TOP` | `OrderBy`, `ThenByDescending`, `Take`, `MaxBy` | 7~9 |
| `EXISTS`, `NOT EXISTS` | `Any`, `All` | 10~12 |
| `SUM`, `AVG` | `Sum`, `Average`, `Aggregate` | 13~15 |
| `GROUP BY` | `GroupBy` + `g.Key`, `g.Count()` | 16~18 |
| `JOIN` | `Join` | 19~20 |

SQL은 `SELECT`를 먼저 쓰지만 LINQ는 데이터가 흐르는 순서(거르기 → 정렬 → 고르기)대로 씁니다.
처음엔 어색했는데, 메서드를 하나씩 이어 붙이면서 중간 결과를 머릿속으로 따라가기에는 오히려 이쪽이 편했습니다.

## 루프와 LINQ 비교

**1번: 가격 20,000원 이상인 책 제목**

```csharp
// foreach
var titles = new List<string>();
foreach (var book in books)
{
    if (book.Price >= 20000)
        titles.Add(book.Title);
}

// LINQ
var titles = books.Where(b => b.Price >= 20000).Select(b => b.Title);
```

**12번: 주문이 한 번도 없는 고객**

```csharp
// foreach
foreach (var customer in customers)
{
    bool hasOrder = false;
    foreach (var order in orders)
    {
        if (order.CustomerId == customer.Id)
        {
            hasOrder = true;
            break;
        }
    }
    if (!hasOrder)
        Console.WriteLine(customer.Name);
}

// LINQ
customers.Where(c => !orders.Any(o => o.CustomerId == c.Id)).Select(c => c.Name);
```

`Any`는 조건에 맞는 항목을 찾는 순간 멈추고 `true`를 반환합니다.
루프 버전의 `hasOrder = true; break;`가 정확히 같은 동작이라는 걸 직접 짜 보고 알게 됐습니다.
LINQ 메서드 하나가 내부에서 어떤 루프를 대신하는지 알고 나니 결과를 예측하기가 쉬워졌습니다.

**16번: 카테고리별 책 수**

```csharp
// foreach + Dictionary
var counts = new Dictionary<string, int>();
foreach (var book in books)
    counts[book.Category] = counts.GetValueOrDefault(book.Category) + 1;

// LINQ
books.GroupBy(b => b.Category).Select(g => $"{g.Key} {g.Count()}");
```

이전 과제(출입 기록 분석기)에서 Dictionary로 직접 집계하던 코드가 `GroupBy` 한 줄이 됐습니다.

## 종합 문제 (20번): 고객별 총 주문 금액

```sql
SELECT c.Name, SUM(b.Price * o.Quantity) AS Total
FROM Orders o
JOIN Books b     ON o.BookId = b.Id
JOIN Customers c ON o.CustomerId = c.Id
GROUP BY c.Name
ORDER BY Total DESC
```

```csharp
orders
    .Join(books,     o => o.BookId,     b => b.Id, (o, b) => new { o.CustomerId, b.Price, o.Quantity })
    .Join(customers, j => j.CustomerId, c => c.Id, (j, c) => new { c.Name, j.Price, j.Quantity })
    .GroupBy(x => x.Name)
    .Select(g => new { Name = g.Key, Total = g.Sum(x => x.Price * x.Quantity) })
    .OrderByDescending(t => t.Total)
    .Select(t => $"{t.Name} {t.Total:N0}");
```

```
홍길동 92,000, 김민수 81,000, 마석도 63,000, 이서연 11,000
```

SQL을 먼저 쓰고 한 줄씩 옮기니 생각보다 어렵지 않았습니다.

## 메서드식과 쿼리식

19번(주문 목록에 고객 이름·책 제목 붙이기)은 쿼리식으로도 작성해 봤습니다.

```csharp
// 메서드식: 다음 Join으로 값을 넘기려고 중간 익명 타입이 필요
orders.Join(customers, o => o.CustomerId, c => c.Id, (o, c) => new { o.Id, c.Name, o.Quantity, o.BookId })
      .Join(books, j => j.BookId, b => b.Id, (j, b) => new { j.Id, j.Name, b.Title, j.Quantity })
      .Select(x => $"주문 {x.Id}: {x.Name} - {x.Title} x{x.Quantity}");

// 쿼리식: SQL과 거의 같은 모양
from o in orders
join c in customers on o.CustomerId equals c.Id
join b in books     on o.BookId     equals b.Id
select $"주문 {o.Id}: {c.Name} - {b.Title} x{o.Quantity}";
```

쿼리식은 컴파일될 때 메서드식으로 바뀌므로 결과와 성능은 같습니다.
Join이 여러 번 들어가는 경우에는 쿼리식이 SQL처럼 읽혀서 편했지만,
`Take`, `Any`, `Sum` 같은 메서드는 쿼리식이 없어서 실무에서는 메서드식을 주로 쓰고 필요하면 섞어 쓰는 것으로 정리했습니다.

## 풀면서 실수한 것들

**1. 루프와 LINQ의 결과가 달랐음 (1번)**

LINQ 버전에 `OrderBy`를 하나 더 넣어서 순서가 달라졌습니다.
"같은 결과를 두 방식으로 낸다"는 게 목적인데, 결과를 나란히 비교하지 않았으면 놓쳤을 실수입니다.

**2. `title`과 `Title` (9번)**

Delphi는 대소문자를 구분하지 않아서 `tt.title`로 썼는데, C#에서는 다른 이름이라 인식되지 않았습니다.
`select`도 같은 이유로 `Select`여야 했습니다.

**3. `continue`와 `break`의 범위 (12번 루프 버전)**

안쪽 루프 안에서 `continue`를 쓰면 바깥 루프의 다음 고객으로 넘어갈 거라고 생각했는데,
`continue`/`break`는 자신을 감싼 **가장 안쪽 반복문**에만 적용됩니다.
그래서 첫 고객이 무조건 출력됐습니다. `hasOrder` 변수로 결과를 기록하는 방식으로 고쳤습니다.

**4. `Aggregate`를 책 목록에 바로 적용 (15번)**

`books.Aggregate((acc, next) => acc + "/" + next)`는 `acc`, `next`가 `Book`이라 문자열을 반환할 수 없었습니다.
`Select(b => b.Title)`로 제목 목록을 먼저 만든 뒤 `Aggregate`를 적용했습니다.

**5. 합계에 수량을 빠뜨림 (20번)**

`g.Sum(x => x.Price)`로 가격만 더해서, 같은 책을 2권 산 주문이 1권 값으로 계산됐습니다. `x.Price * x.Quantity`로 고쳤습니다.

**6. 정렬 위치 (20번)**

처음엔 결과를 문자열로 만든 다음 정렬하려고 했는데, 문자열이 되면 숫자 크기가 아니라 글자 순서로 정렬됩니다.
**숫자로 계산 → 정렬 → 마지막에 문자열로 변환** 순서로 바꿨습니다. 그래서 중간 단계를 익명 타입 `new { Name, Total }`으로 담아 두었습니다.

## 알아 둘 것

- **지연 실행**: `books.Where(...)`는 조건을 적어 둔 것이고, `foreach`나 `ToList()`를 할 때 실행됩니다.
- **값 하나를 반환하는 메서드**: `First`, `MaxBy`, `Sum`, `Any`, `Aggregate`는 목록이 아니라 값을 반환합니다.
- **빈 결과 처리**: `First()`는 결과가 없으면 예외, `FirstOrDefault()`와 `MaxBy()`는 `null`을 반환합니다. 그래서 `MaxBy`의 결과는 `Book?`이고 null 확인이 필요합니다.
- **람다 변수 이름**: 항목은 `x`, 그룹은 `g`, 집계 결과는 `t`처럼 나눠 쓰면 각 단계에서 무엇을 다루는지 바로 보입니다.

## 다음에 해 볼 것

- 출입 기록 분석기(List/Dictionary/HashSet 과제)를 LINQ로 다시 작성해서 코드 길이 비교
- 20번을 쿼리식(`group ... by ... into g`)으로도 작성
- 데이터가 많을 때 `Any` 이중 루프 대신 `HashSet`으로 바꾸는 방법 확인
