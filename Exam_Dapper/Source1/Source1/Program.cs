

using Dapper;
using Microsoft.Data.Sqlite;

using Microsoft.Extensions.Configuration;
using System.Text;

// ===== Connection =====
IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)                       // 실행 폴더 기준
    .AddJsonFile("appsettings.json", optional: false)            // 없으면 오류
    .Build();

string connStr = config.GetConnectionString("Equipment")
    ?? throw new InvalidOperationException("ConnectionStrings:Equipment 설정이 없습니다.");


using var conn = new SqliteConnection(connStr);
conn.Open();

// ===== step1 ===== 
await Step1();

//===== step2 =====
await Step2();

//==== step3 =====
var equipment = await GetByIdAsync(9999);
Console.WriteLine(equipment is null ? "없음" : $"id : {equipment.Id} name : {equipment.Name} status :{equipment.Status} " +
                                               $"on : {equipment.PurchasedOn} note : {equipment.Note} CATEGORY : {equipment.Category}");

var items = await SearchAsync("노트북", "가전", EquipmentStatus.Available.ToString());
foreach (var item in items) {
    Console.WriteLine($"id : {item.Id} name : {item.Name} status :{item.Status} on : {item.PurchasedOn} note : {item.Note} CATEGORY : {item.Category}");
}

await PrintPage(1, 10);   // 1/3, 10건
await PrintPage(2, 10);   // 2/3, 10건, 1페이지와 겹치지 않음
await PrintPage(3, 10);   // 3/3, 6건 (총 26건 기준)
await PrintPage(4, 10);   // 범위 밖: 0건, 총 26건 유지
await PrintPage(0, 10);   // 1페이지로 보정
await PrintPage(1, 0);    // 예외
await PrintPage(1, -5);   // 예외


async Task Step1()
{

    using var cmd = conn.CreateCommand();
    cmd.CommandText = "DROP TABLE IF EXISTS EQUIPMENT";
    cmd.ExecuteNonQuery();

    cmd.CommandText = """
    CREATE TABLE IF NOT EXISTS EQUIPMENT (
        ID          INTEGER PRIMARY KEY AUTOINCREMENT,
        NAME        TEXT NOT NULL,
        STATUS      TEXT NOT NULL,
        PURCHASEDON TEXT,
        NOTE        TEXT,
        CATEGORY    TEXT
    );
    """;
    cmd.ExecuteNonQuery();

    cmd.CommandText = "delete from EQUIPMENT";
    cmd.ExecuteNonQuery();

    long count = await conn.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM Equipment");

    if (count == 0)
    {
        cmd.CommandText = "INSERT INTO EQUIPMENT (NAME, STATUS, PURCHASEDON, NOTE, CATEGORY) VALUES ($NAME, $STATUS, $ON, $NOTE, $CATEGORY)";
        var pName = cmd.Parameters.Add("$NAME", SqliteType.Text);
        var pStatus = cmd.Parameters.Add("$STATUS", SqliteType.Text);
        var pOn = cmd.Parameters.Add("$ON", SqliteType.Text);
        var pNote = cmd.Parameters.Add("$NOTE", SqliteType.Text);
        var pCategroy = cmd.Parameters.Add("$CATEGORY", SqliteType.Text);

        for (int i = 1; i <= 25; i++) {
            Insert($"비품{i}", EquipmentStatus.Available.ToString(), "2029-11-01", "test", "가전");
        }
        //Insert("노트북1", EquipmentStatus.Available.ToString(), "2029-11-01", "test", "가전");
        //Insert("노트북2", EquipmentStatus.Available.ToString(), "2029-11-02", null, "가전");
        //Insert("침대", EquipmentStatus.Broken.ToString(), "2029-11-03", null, "가구");

        void Insert(string name, string status, string? on, string? note, string category)
        {
            pName.Value = name;
            pStatus.Value = status;
            if (on is null)
            {
                pOn.Value = DBNull.Value;
            }
            else
            {
                pOn.Value = on;
            }

            if (note is null)
            {
                pNote.Value = DBNull.Value;
            }
            else
            {
                pNote.Value = note;
            }
            pCategroy.Value = category;
            cmd.ExecuteNonQuery();
        }
    }

    //var list = (await conn.QueryAsync<Equipment>("SELECT Id, Name, Status, PurchasedOn, Note, CATEGORY FROM Equipment")).ToList();

    //foreach (var item in list)
    //{
    //    Console.WriteLine($"id : {item.Id} name : {item.Name} status :{item.Status} on : {item.PurchasedOn} note : {item.Note} CATEGORY : {item.Category}");
    //}
}


async Task Step2()
{
    var item = new Equipment { Name = "프린터", Status = EquipmentStatus.Available, PurchasedOn = DateTime.Parse("2026-01-01"), Note = "test" , Category = "사무"};
    string inststr = """
                        INSERT INTO EQUIPMENT (NAME, STATUS, PURCHASEDON, NOTE, CATEGORY)
                        VALUES (@Name, @Status, @PurchasedOn, @Note, @CATEGORY)
                        RETURNING ID;
                    """;
    long id = await conn.QuerySingleAsync<long>(inststr, new {item.Name, Status = item.Status.ToString(),PurchasedOn = item.PurchasedOn?.ToString("yyyy-MM-dd"), item.Note,
                                                              CATEGORY = item.Category});

    string sql = "SELECT * FROM EQUIPMENT where ID = @id";
    var list = (await conn.QueryAsync<Equipment>(sql, new {id})).ToList();
    foreach(var item2 in list)
    {
        Console.WriteLine($"id : {item2.Id} name : {item2.Name} status :{item2.Status} on : {item2.PurchasedOn} note : {item2.Note} CATEGORY : {item2.Category}");
    }
}

async Task<Equipment?> GetByIdAsync(int id)
{
    string sql = " SELECT * FROM EQUIPMENT WHERE ID = @ID ";
    return await conn.QuerySingleOrDefaultAsync<Equipment>(sql, new { ID = id });
}

async Task<List<Equipment>> SearchAsync(string keyWord, string cateGory, string status) {
    var sql = new StringBuilder(" SELECT * FROM EQUIPMENT WHERE 1=1 ");
    var param = new DynamicParameters();

    if (!string.IsNullOrWhiteSpace(keyWord)) {
        sql.Append(" AND NAME LIKE @keyWord ");
        param.Add("keyWord", $"%{keyWord}%");
    }

    if (!string.IsNullOrWhiteSpace(cateGory))
    {
        sql.Append(" AND CATEGORY LIKE @cateGory ");
        param.Add("cateGory", cateGory);
    }

    if (!string.IsNullOrWhiteSpace(status))
    {
        sql.Append(" AND STATUS LIKE @status ");
        param.Add("status", status);
    }

    sql.Append(" ORDER BY ID ");
    return (await conn.QueryAsync<Equipment>(sql.ToString(), param)).ToList();
}

async Task<PagedResult<Equipment>> GetPage(int page, int pageSize) {
    page = Math.Max(page, 1);

    if (pageSize <= 0) { throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, "pageSize는 1 이상이어야 합니다."); }

    string sql = " SELECT * FROM EQUIPMENT ORDER BY ID LIMIT @PAGESIZE OFFSET @OFFSET";
    var offsetList = (await conn.QueryAsync<Equipment>(sql, new { PAGESIZE = pageSize, OFFSET = (page - 1) * pageSize })).ToList();

    sql = " SELECT COUNT(*) FROM EQUIPMENT ";
    long total = await conn.QuerySingleAsync<long>(sql);
    return new PagedResult<Equipment>(offsetList, (int)total, page, pageSize);        
}

async Task PrintPage(int page, int pageSize)
{
    try
    {
        var r = await GetPage(page, pageSize);
        Console.WriteLine($"\nGetPage({page}, {pageSize}) → [{r.Page}/{r.TotalPages} 페이지] 총 {r.TotalCount}건, 이번 페이지 {r.Items.Count}건");
        foreach (var item in r.Items)
            Console.WriteLine($"  id : {item.Id} name : {item.Name} status : {item.Status} on : {item.PurchasedOn} note : {item.Note} CATEGORY : {item.Category}");
    }
    catch (ArgumentOutOfRangeException ex)
    {
        Console.WriteLine($"\nGetPage({page}, {pageSize}) → 예외: {ex.Message}");
    }
}


public enum EquipmentStatus { Available = 0, OnLoan = 1, Broken = 2 };
class Equipment
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public EquipmentStatus Status { get; set; }
    public DateTime? PurchasedOn { get; set; }
    public string? Note { get; set; }
    public string Category { get; set; } = "";
}

record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}