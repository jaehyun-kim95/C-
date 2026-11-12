# 사내 비품 관리 - Dapper로 Repository 작성 (CRUD)

## 목적

DB 접근 코드(SQL, 연결, 매핑)를 한 클래스에 모으고, 나머지 코드는 그 클래스의 **약속(인터페이스)만** 알게 만드는 것이 목표다.
이를 위해 마이크로 ORM인 **Dapper**로 SQLite의 비품 테이블에 CRUD를 구현한다.

| 개념 | 한 줄 정의 |
|---|---|
| Dapper | SQL은 직접 쓰고, 결과를 객체로 바꾸는 일(매핑)만 대신해 주는 라이브러리. `IDbConnection`의 확장 메서드로 동작한다. |
| Repository | 데이터 저장소 접근을 한 클래스에 모은 것. SQL은 이 클래스 안에만 있다. |
| 파라미터 쿼리 | 값은 SQL 문자열에 끼워 넣지 않고 `@이름`으로 넘긴다. SQL 인젝션을 막는 기본 규칙. |
| 영향받은 행 수 | `UPDATE`/`DELETE`는 대상이 없어도 에러가 나지 않는다. 실제로 바뀌었는지는 `Execute`가 돌려주는 행 수로 판단한다. |
| 인터페이스 | 호출하는 쪽은 `IEquipmentRepository`만 안다. 구현(Dapper -> EF Core 등)이 바뀌어도 호출부는 그대로. |

## Dapper

### Dapper란

- Stack Overflow 개발팀이 만든 오픈소스 **마이크로 ORM**. NuGet 패키지 `Dapper` 하나로 쓴다.
- ADO.NET의 `IDbConnection`에 **확장 메서드를 붙여 주는 얇은 라이브러리**다. 연결은 사용하는 DB의 드라이버(이 실습에서는 `Microsoft.Data.Sqlite`)를 그대로 쓴다.
- 해 주는 일은 딱 두 가지다.
  1. 파라미터 바인딩 (`@Id` <- C# 값)
  2. 결과 행을 객체로 매핑 (컬럼 -> 속성)
- 하지 않는 일: SQL 생성, 변경 추적, 마이그레이션. SQL은 직접 쓴다.
- Delphi로 비유하면 `Query.SQL.Text` + `ParamByName(...)` + 필드를 읽어 객체에 옮기는 반복 코드 중, **파라미터와 필드 옮기기를 자동화**해 주는 도구다.

### 왜 쓰는가

ADO.NET만 쓰면 컬럼마다 값을 꺼내 속성에 옮기는 코드를 매번 써야 한다.

```csharp
// ADO.NET
using var cmd = conn.CreateCommand();
cmd.CommandText = "SELECT ID, NAME, STATUS FROM EQUIPMENT WHERE ID = $id";
cmd.Parameters.AddWithValue("$id", id);
using var reader = await cmd.ExecuteReaderAsync();
Equipment? item = null;
if (await reader.ReadAsync())
    item = new Equipment {
        Id = reader.GetInt32(0),
        Name = reader.GetString(1),
        Status = Enum.Parse<EquipmentStatus>(reader.GetString(2))
        // 컬럼이 늘 때마다 한 줄씩 추가
    };

// Dapper
var item = await conn.QuerySingleOrDefaultAsync<Equipment>(
    "SELECT ID, NAME, STATUS FROM EQUIPMENT WHERE ID = @Id", new { Id = id });
```

| | ADO.NET | Dapper | EF Core |
|---|---|---|---|
| SQL | 직접 | 직접 | LINQ -> 자동 생성 |
| 결과 -> 객체 | 직접 | 자동 | 자동 |
| 변경 추적 / 마이그레이션 | 없음 | 없음 | 있음 |
| 코드량 | 많음 | 적음 | 가장 적음 |
| 성능 | 기준 | ADO.NET에 가까움 | 상대적으로 무거움 |
| 학습 부담 | 낮음 | 낮음 | 높음 |

- SQL을 직접 통제하고 싶을 때, 조회 중심 화면, 기존 SQL 자산이 많을 때 Dapper가 맞다.
- 객체 변경을 자동 저장하거나 스키마 마이그레이션까지 맡기고 싶다면 EF Core가 맞다.

### 문법

**메서드** (모두 `...Async` 버전이 있다)

| 메서드 | 용도 | 반환 | 결과가 없거나 여러 건일 때 |
|---|---|---|---|
| `Query<T>` | 여러 행 조회 | `IEnumerable<T>` | 없으면 빈 목록 |
| `QueryFirst<T>` / `QueryFirstOrDefault<T>` | 첫 행 | `T` / `T?` | 없으면 예외 / `null`, 여러 건이어도 첫 행 |
| `QuerySingle<T>` / `QuerySingleOrDefault<T>` | 정확히 1행 | `T` / `T?` | 0건이면 예외 / `null`, **2건 이상이면 예외** |
| `Execute` | INSERT / UPDATE / DELETE | 영향받은 행 수 `int` | |
| `ExecuteScalar<T>` | 첫 행 첫 컬럼 하나 (`COUNT(*)` 등) | `T` | |
| `QueryMultiple` | SELECT 여러 개를 한 번에 | `GridReader` | |

Id로 한 건을 찾을 때는 `QuerySingleOrDefault`가 맞다. 2건 이상이면 데이터가 잘못된 것이므로 예외로 드러내는 편이 낫다.

**파라미터 넘기기**

```csharp
// 1) 익명 객체: 가장 흔한 형태. 속성 이름이 @이름과 일치해야 한다.
await conn.QueryAsync<Equipment>("SELECT ... WHERE CATEGORY = @Category", new { Category = "가전" });

// 2) 객체 그대로: 속성 이름으로 매칭된다. (값 변환이 필요한 속성은 익명 객체로 따로 지정)
await conn.ExecuteAsync("UPDATE ... WHERE ID = @Id", item);

// 3) DynamicParameters: 조건을 동적으로 붙일 때
var p = new DynamicParameters();
p.Add("Keyword", $"%{keyword}%");

// 4) IN 목록: 배열을 넘기면 알아서 펼쳐진다.
await conn.QueryAsync<Equipment>("SELECT ... WHERE ID IN @Ids", new { Ids = new[] { 1, 2, 3 } });
```

**매핑 규칙**

- 컬럼 이름과 속성 이름이 **대소문자 무시**로 같으면 채운다. 짝이 없는 컬럼은 에러 없이 건너뛴다.
- 문자열 컬럼은 `enum`(`STATUS` -> `EquipmentStatus`), `DateTime`으로 변환된다. 변환할 수 없는 값이면 예외가 난다.
- 컬럼이 `NULL`이면 속성은 `?` 타입(`DateTime?`, `string?`)이어야 한다.
- 컬럼 이름이 속성과 다르면 SQL에서 `AS`로 맞춘다. (`SELECT BORROWER AS Borrower`)

**연결과 트랜잭션, 취소**

```csharp
using var conn = new SqliteConnection(connStr);   // 연결은 using으로 직접 만들고 닫는다
// Dapper는 닫혀 있는 연결을 알아서 열고, 끝나면 다시 닫는다.

await conn.ExecuteAsync(sql, param, transaction: tx);                          // 트랜잭션
await conn.QueryAsync<T>(new CommandDefinition(sql, param, cancellationToken: ct)); // 취소 토큰
```

**주의할 점**

- Dapper는 SQL을 검사하지 않는다. 오타는 **실행할 때** 에러가 나고, 컬럼/속성 이름 불일치는 에러 없이 기본값으로 남는다. 그래서 호출해서 값을 확인하는 단계가 필요하다.
- 값은 항상 파라미터로 넘긴다. 테이블명, 컬럼명, 정렬 방향은 파라미터로 쓸 수 없으므로 정해진 목록에서만 고르게(화이트리스트) 한다.

## 구성

| 파일 | 역할 |
|---|---|
| `Equipment.cs` | 비품 모델, `EquipmentStatus` 열거형 |
| `PagedResult.cs` | 페이지 결과(목록 + 전체 건수) |
| `IEquipmentRepository.cs` | 저장소의 약속 |
| `EquipmentRepository.cs` | Dapper 구현 (SQL은 여기에만) |
| `DbInitializer.cs` | 테이블 생성과 시드 데이터 |
| `Program.cs` | 설정 읽기, 객체 생성, 호출 확인 |

연결 문자열은 `appsettings.json`에서 읽는다. 저장소 메서드는 호출할 때마다 연결을 열고 닫으며, 모두 `async` + `CancellationToken`을 받는다.

테이블:

```sql
CREATE TABLE EQUIPMENT (
    ID          INTEGER PRIMARY KEY AUTOINCREMENT,
    NAME        TEXT NOT NULL,
    STATUS      TEXT NOT NULL,      -- Available / OnLoan / Broken (enum을 문자열로 저장)
    PURCHASEDON TEXT,               -- yyyy-MM-dd
    NOTE        TEXT,
    CATEGORY    TEXT
);
```

## 샘플 case

### 1. 조회

```csharp
// 한 건: 없으면 null
var item = await repo.GetByIdAsync(10);

// 조건 검색: 값이 있는 조건만 AND로 붙인다
var list = await repo.SearchAsync(keyword: "노트북", category: "가전", status: EquipmentStatus.Available);
```

```csharp
// 저장소 안쪽: 값은 항상 파라미터
conn.QuerySingleOrDefaultAsync<Equipment>("SELECT ... FROM EQUIPMENT WHERE ID = @Id", new { Id = id });
```

### 2. 등록 (새 Id 받기)

```csharp
int newId = await repo.AddAsync(new Equipment { Name = "프린터", Status = EquipmentStatus.Available, Category = "사무" });
```

```sql
INSERT INTO EQUIPMENT (NAME, STATUS, PURCHASEDON, NOTE, CATEGORY)
VALUES (@Name, @Status, @PurchasedOn, @Note, @Category)
RETURNING ID;
```

### 3. 수정 / 삭제 (영향받은 행 수로 판단)

```csharp
bool updated = await repo.UpdateAsync(item);   // 없는 Id면 false (예외 아님)
bool deleted = await repo.DeleteAsync(10);     // 이미 지워졌으면 false
```

```csharp
int row = await conn.ExecuteAsync("DELETE FROM EQUIPMENT WHERE ID = @Id", new { Id = id });
return row == 1;
```

`UPDATE`/`DELETE`의 `WHERE`를 빠뜨리면 전체 행이 바뀌므로, "다른 행은 그대로인가"를 꼭 확인한다.

### 4. 페이징 (LIMIT / OFFSET + 전체 건수)

```csharp
var page = await repo.GetPageAsync(page: 2, pageSize: 10);
// page.Items: 이번 페이지 행, page.TotalCount: 전체 건수, page.TotalPages: 올림(전체 / 페이지 크기)
```

- `OFFSET = (page - 1) * pageSize`, 정렬 기준(`ORDER BY ID`)이 있어야 페이지 사이가 겹치지 않는다.
- 페이지 번호는 1 미만이면 1로 보정하고, 페이지 크기가 1 미만이면 예외로 막는다.

### 5. 호출하는 쪽 (구현을 모른다)

```csharp
IEquipmentRepository repo = new EquipmentRepository(connStr);   // 구현을 고르는 곳은 이 한 줄
var item = await repo.GetByIdAsync(10);
```

## 참고 (직접 구현하지 않은 부분)

- **트랜잭션**: 여러 SQL을 "전부 성공하거나 전부 취소"로 묶는 장치. 대여 처리(비품 상태 변경 + 이력 추가)가 대표 사례다. `BeginTransaction` -> 모든 SQL에 `transaction: tx` -> `Commit`, 그리고 `Commit` 없이 끝나면 자동 롤백된다. 트랜잭션은 연결 하나에 속한다는 점, `Commit`을 잊으면 에러 없이 저장되지 않는다는 점만 기억해 둔다.
- **JOIN + multi-mapping**: 두 테이블을 한 번에 읽어 객체 두 개로 나눠 담는다 (`splitOn`). `LEFT JOIN`의 조건은 `ON`과 `WHERE`가 다르게 동작하고, NULL은 `= NULL`이 아니라 `IS NULL`로 비교한다.
- 실제 사용법은 개발하면서 필요할 때 찾아서 쓴다.
