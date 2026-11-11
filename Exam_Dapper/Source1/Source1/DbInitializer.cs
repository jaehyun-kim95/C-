
using Dapper;
using Microsoft.Data.Sqlite;

namespace Source1
{
    internal class DbInitializer(string connStr)
    {
        public async Task CreateTable()
        {
            using var conn = new SqliteConnection(connStr);

            await conn.ExecuteAsync("DROP TABLE IF EXISTS EQUIPMENT");
            await conn.ExecuteAsync("""
                                CREATE TABLE IF NOT EXISTS EQUIPMENT (
                                    ID          INTEGER PRIMARY KEY AUTOINCREMENT,
                                    NAME        TEXT NOT NULL,
                                    STATUS      TEXT NOT NULL,
                                    PURCHASEDON TEXT,
                                    NOTE        TEXT,
                                    CATEGORY    TEXT
                                );
                                """);
        }

        public async Task InstSeedData()
        {
            using var conn = new SqliteConnection(connStr);
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();

            cmd.CommandText = "INSERT INTO EQUIPMENT (NAME, STATUS, PURCHASEDON, NOTE, CATEGORY) VALUES ($NAME, $STATUS, $ON, $NOTE, $CATEGORY)";
            var pName = cmd.Parameters.Add("$NAME", SqliteType.Text);
            var pStatus = cmd.Parameters.Add("$STATUS", SqliteType.Text);
            var pOn = cmd.Parameters.Add("$ON", SqliteType.Text);
            var pNote = cmd.Parameters.Add("$NOTE", SqliteType.Text);
            var pCategroy = cmd.Parameters.Add("$CATEGORY", SqliteType.Text);

            for (int i = 1; i <= 25; i++) { Insert($"비품{i}", EquipmentStatus.Available.ToString(), "2029-11-01", "test", "가전"); }

            void Insert(string name, string status, string? on, string? note, string category) {
                pName.Value = name;
                pStatus.Value = status;

                if (on is null) {
                    pOn.Value = DBNull.Value;
                } else {
                    pOn.Value = on;
                }

                if (note is null) {
                    pNote.Value = DBNull.Value;
                } else {
                    pNote.Value = note;
                }

                pCategroy.Value = category;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
