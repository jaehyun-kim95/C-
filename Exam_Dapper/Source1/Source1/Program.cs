

using Microsoft.Extensions.Configuration;
using Source1;

// ===== Connection =====
IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)                       // 실행 폴더 기준
    .AddJsonFile("appsettings.json", optional: false)            // 없으면 오류
    .Build();

string connStr = config.GetConnectionString("Equipment")
    ?? throw new InvalidOperationException("ConnectionStrings:Equipment 설정이 없습니다.");

IEquipmentRepository repo = new EquipmentRepository(connStr);

int failed = 0;
void Check(bool ok, string name)
{
    Console.WriteLine($"{(ok ? "PASS" : "FAIL")}  {name}");
    if (!ok) failed++;
}
async Task<bool> IsCanceled(Func<Task> act)
{
    try { await act(); return false; }
    catch (OperationCanceledException) { return true; }   // TaskCanceledException도 여기에 잡힘
}
async Task<int> Total() => (await repo.GetPageAsync(1, 10)).TotalCount;

// ===== 기준: 시드 25건 =====
Check(await Total() == 25, "기준: 시드 25건");

// ===== 페이징 (3단계 복습) =====
var p1 = await repo.GetPageAsync(1, 10);
var p2 = await repo.GetPageAsync(2, 10);
var p3 = await repo.GetPageAsync(3, 10);
var p4 = await repo.GetPageAsync(4, 10);
Check(p1.Items.Count == 10 && p1.TotalPages == 3, "페이징: 1페이지 10건, 총 3페이지");
Check(p2.Items[0].Id == p1.Items[^1].Id + 1, "페이징: 1, 2페이지가 겹치지 않고 이어진다");
Check(p3.Items.Count == 5, "페이징: 마지막 페이지는 5건");
Check(p4.Items.Count == 0 && p4.TotalCount == 25, "페이징: 범위 밖은 빈 목록, 총 건수 유지");
Check((await repo.GetPageAsync(0, 10)).Page == 1, "페이징: page 0은 1페이지로 보정");
Check(await IsCanceled(() => repo.GetPageAsync(1, 0)) == false
      && await Throws<ArgumentOutOfRangeException>(() => repo.GetPageAsync(1, 0)), "페이징: pageSize 0은 예외");

async Task<bool> Throws<T>(Func<Task> act) where T : Exception
{
    try { await act(); return false; }
    catch (T) { return true; }
}

// ===== 2. Add → GetById =====
var date = new DateTime(2026, 1, 5);
int newId = await repo.AddAsync(new Equipment
{
    Name = "TEST_노트북",
    Status = EquipmentStatus.OnLoan,
    PurchasedOn = date,
    Note = "메모",
    Category = "TEST_가전"
});
var got = await repo.GetByIdAsync(newId);
Check(newId > 25, "2. Add: 새 Id가 시드 뒤 번호");
Check(got is not null && got.Name == "TEST_노트북" && got.Status == EquipmentStatus.OnLoan
      && got.PurchasedOn == date && got.Note == "메모" && got.Category == "TEST_가전",
      "2. Add 후 GetById: 넣은 값 그대로");

// ===== 3. Update: 대상만 바뀌고 다른 행은 그대로 =====
var otherBefore = await repo.GetByIdAsync(5);
bool updated = await repo.UpdateAsync(new Equipment
{
    Id = newId,
    Name = "TEST_노트북2",
    Status = EquipmentStatus.Available,
    PurchasedOn = new DateTime(2026, 2, 1),
    Note = "수정",
    Category = "TEST_가전"
});
var after = await repo.GetByIdAsync(newId);
var otherAfter = await repo.GetByIdAsync(5);
Check(updated && after is not null && after.Name == "TEST_노트북2" && after.Status == EquipmentStatus.Available
      && after.Note == "수정", "3. Update: 대상 행이 바뀜");
Check(otherBefore is not null && otherAfter is not null
      && otherBefore.Name == otherAfter.Name && otherBefore.Status == otherAfter.Status
      && otherBefore.Note == otherAfter.Note && otherBefore.Category == otherAfter.Category
      && otherBefore.PurchasedOn == otherAfter.PurchasedOn, "3. Update: 다른 행(5번)은 그대로");

// ===== 4. 없는 Id =====
Check(await repo.UpdateAsync(new Equipment { Id = 9999, Name = "없음" }) == false, "4. 없는 Id Update → false");
Check(await repo.DeleteAsync(9999) == false, "4. 없는 Id Delete → false");

// ===== 5, 6, 7. Delete =====
int before = await Total();                       // 26
Check(await repo.DeleteAsync(newId), "5. Delete → true");
Check(await repo.GetByIdAsync(newId) is null, "5. 삭제 후 GetById → null");
Check(await repo.DeleteAsync(newId) == false, "6. 같은 Id 다시 Delete → false");
Check(await Total() == before - 1, "7. 총 건수가 1 줄어든다");

// ===== 8. Search 전체 =====
var all = await repo.SearchAsync(null, null, null);
Check(all.Count == await Total(), "8. Search(null, null, null)은 전체");
Check(all.Select(x => x.Id).SequenceEqual(all.Select(x => x.Id).OrderBy(i => i)), "8. Id 순서");

// ===== 9. Search 조합 (테스트용 3건 추가) =====
int r1 = await repo.AddAsync(new Equipment { Name = "TEST_의자", Status = EquipmentStatus.Available, Category = "TEST_가구" });
int r2 = await repo.AddAsync(new Equipment { Name = "TEST_책상", Status = EquipmentStatus.OnLoan, Category = "TEST_가구" });
int r3 = await repo.AddAsync(new Equipment { Name = "TEST_모니터", Status = EquipmentStatus.OnLoan, Category = "TEST_가전" });

Check((await repo.SearchAsync("TEST", null, null)).Count == 3, "9. keyword만 → 3");
Check((await repo.SearchAsync("TEST", "TEST_가구", null)).Count == 2, "9. keyword + category → 2");
Check((await repo.SearchAsync(null, "TEST_가구", EquipmentStatus.OnLoan)).Count == 1, "9. category + status → 1");
Check((await repo.SearchAsync(null, null, EquipmentStatus.OnLoan)).Count == 2, "9. status만 → 2");
Check((await repo.SearchAsync("없는이름", null, null)).Count == 0, "9. 일치 없음 → 0");
Check((await repo.SearchAsync(null, "TEST_가%", null)).Count == 0, "9. category는 와일드카드가 아니라 정확히 일치(=)");

foreach (var id in new[] { r1, r2, r3 }) await repo.DeleteAsync(id);   // 정리
Check(await Total() == 25, "정리 후 다시 25건");

// ===== 10. 취소 토큰 =====
var canceled = new CancellationToken(true);
int totalBeforeCancel = await Total();
Check(await IsCanceled(() => repo.GetByIdAsync(1, canceled)), "10. GetById 취소");
Check(await IsCanceled(() => repo.GetPageAsync(1, 10, canceled)), "10. GetPage 취소");
Check(await IsCanceled(() => repo.SearchAsync(null, null, null, canceled)), "10. Search 취소");
Check(await IsCanceled(() => repo.AddAsync(new Equipment { Name = "X" }, canceled)), "10. Add 취소");
Check(await IsCanceled(() => repo.UpdateAsync(new Equipment { Id = 1, Name = "X" }, canceled)), "10. Update 취소");
Check(await IsCanceled(() => repo.DeleteAsync(1, canceled)), "10. Delete 취소");
Check(await Total() == totalBeforeCancel && await repo.GetByIdAsync(1) is not null, "10. 취소된 Add/Delete는 DB를 바꾸지 않았다");

Console.WriteLine(failed == 0 ? "\n모두 통과" : $"\n실패 {failed}건");
return failed;


//var DbInit = new DbInitializer(connStr);
//await DbInit.CreateTable();
//await DbInit.InstSeedData();

//IEquipmentRepository repo = new EquipmentRepository(connStr);
//var item = await repo.GetByIdAsync(10);

//Console.WriteLine(item is null ? "없음" : $"id : {item.Id} name : {item.Name} status :{item.Status} " +
//                                          $"on : {item.PurchasedOn} note : {item.Note} CATEGORY : {item.Category}");
