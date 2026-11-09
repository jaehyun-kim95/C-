

using Dapper;
using Microsoft.Data.Sqlite;

using Microsoft.Extensions.Configuration;

IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)                       // 실행 폴더 기준
    .AddJsonFile("appsettings.json", optional: false)            // 없으면 오류
    .Build();

string connStr = config.GetConnectionString("Equipment")
    ?? throw new InvalidOperationException("ConnectionStrings:Equipment 설정이 없습니다.");


using var conn = new SqliteConnection(connStr);
conn.Open();

using var cmd = conn.CreateCommand();
cmd.CommandText = """
    CREATE TABLE IF NOT EXISTS EQUIPMENT (
        ID          INTEGER PRIMARY KEY AUTOINCREMENT,
        NAME        TEXT NOT NULL,
        STATUS      TEXT NOT NULL,
        PURCHASEDON TEXT,
        NOTE        TEXT
    );
    """;
cmd.ExecuteNonQuery();

cmd.CommandText = "delete from EQUIPMENT";
cmd.ExecuteNonQuery();

long count = await conn.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM Equipment");

if (count == 0)
{
    cmd.CommandText = "INSERT INTO EQUIPMENT (NAME, STATUS, PURCHASEDON, NOTE) VALUES ($NAME, $STATUS, $ON, $NOTE)";
    var pName = cmd.Parameters.Add("$NAME", SqliteType.Text);
    var pStatus = cmd.Parameters.Add("$STATUS", SqliteType.Text);
    var pOn = cmd.Parameters.Add("$ON", SqliteType.Text);
    var pNote = cmd.Parameters.Add("$NOTE", SqliteType.Text);

    void Insert(string name, string status, string? on, string? note)
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
        cmd.ExecuteNonQuery();
    }

    Insert("노트북", EquipmentStatus.Available.ToString(), "2029-11-01", "test");
    Insert("TV", EquipmentStatus.OnLoan.ToString(), "2029-11-02", null);
    Insert("침대", EquipmentStatus.Broken.ToString(), "2029-11-03", null);
}

var list = (await conn.QueryAsync<Equipment>("SELECT Id, Name, Status, PurchasedOn, Note FROM Equipment")).ToList();

conn.Close();

foreach(var item in list)
{
    Console.WriteLine($"id : {item.Id} name : {item.Name} status :{item.Status} on : {item.PurchasedOn} note : {item.Note}");
}


public enum EquipmentStatus { Available = 0, OnLoan = 1, Broken = 2 };
class Equipment
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public EquipmentStatus Status { get; set; }
    public DateTime? PurchasedOn { get; set; }
    public string? Note { get; set; }
}