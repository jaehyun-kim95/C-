using Dapper;
using Microsoft.Data.Sqlite;
using System.Net.NetworkInformation;
using System.Text;

namespace Source1
{
    internal class EquipmentRepository(string connStr) : IEquipmentRepository
    {

        // ==== INSERT ===== 
        public async Task<int> AddAsync(Equipment item, CancellationToken ct)
        {
            using var conn = new SqliteConnection(connStr);
            string sql = """
                        INSERT INTO EQUIPMENT (NAME, STATUS, PURCHASEDON, NOTE, CATEGORY)
                        VALUES (@Name, @Status, @PurchasedOn, @Note, @CATEGORY)
                        RETURNING ID;
                    """;
            return await conn.QuerySingleAsync<int>(new CommandDefinition(sql,
                                                                    new
                                                                    {
                                                                        item.Name,
                                                                        Status = item.Status.ToString(),
                                                                        PurchasedOn = item.PurchasedOn?.ToString("yyyy-MM-dd"),
                                                                        item.Note,
                                                                        CATEGORY = item.Category
                                                                    }, cancellationToken: ct));           
        }

        // ===== DELETE ===== 
        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            using var conn = new SqliteConnection(connStr);
            int row = await conn.ExecuteAsync(new CommandDefinition("DELETE FROM EQUIPMENT WHERE ID = @ID ", new { ID = id }, cancellationToken: ct));
            return row == 1;
        }

        // ===== SELECT ONE ID ===== 
        public async Task<Equipment?> GetByIdAsync(int id, CancellationToken ct)
        {
            using var conn = new SqliteConnection(connStr);
            string sql = " SELECT * FROM EQUIPMENT WHERE ID = @ID ";
            return await conn.QuerySingleOrDefaultAsync<Equipment>(new CommandDefinition(sql, new { ID = id }, cancellationToken: ct));
        }

        // ===== SELECT Total Page =====
        public async Task<PagedResult<Equipment>> GetPageAsync(int page, int pageSize, CancellationToken ct)
        {
            using var conn = new SqliteConnection(connStr);
            page = Math.Max(page, 1);

            //if (pageSize <= 0) { throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, "pageSize는 1 이상이어야 합니다."); }

            string sql = " SELECT * FROM EQUIPMENT ORDER BY ID LIMIT @PAGESIZE OFFSET @OFFSET";
            var offsetList = (await conn.QueryAsync<Equipment>(new CommandDefinition(sql, new { PAGESIZE = pageSize, OFFSET = (page - 1) * pageSize }, cancellationToken: ct)) ).ToList();

            sql = " SELECT COUNT(*) FROM EQUIPMENT ";
            long total = await conn.QuerySingleAsync<long>(new CommandDefinition(sql, cancellationToken: ct));
            return new PagedResult<Equipment>(offsetList, (int)total, page, pageSize);
        }

        // ===== 특정 row SELECT ===== 
        public async Task<IReadOnlyList<Equipment>> SearchAsync(string? keyword, string? category, EquipmentStatus? status, CancellationToken ct)
        {
            using var conn = new SqliteConnection(connStr);
            var sql = new StringBuilder(" SELECT * FROM EQUIPMENT WHERE 1=1 ");
            var param = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql.Append(" AND NAME LIKE @keyWord ");
                param.Add("keyWord", $"%{keyword}%");
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                sql.Append(" AND CATEGORY = @cateGory ");
                param.Add("cateGory", category);
            }

            if (status is not null)
            {
                sql.Append(" AND STATUS = @status ");
                param.Add("status", status.Value.ToString());
            }

            sql.Append(" ORDER BY ID ");
            return (await conn.QueryAsync<Equipment>(new CommandDefinition(sql.ToString(), param, cancellationToken : ct))).ToList();
        }

        // ===== UPDATE ===== 
        public async Task<bool> UpdateAsync(Equipment item, CancellationToken ct)
        {
            using var conn = new SqliteConnection(connStr);
            string sql = " UPDATE EQUIPMENT                  " +
                         "    SET NAME = @NAME               " +
                         "       ,STATUS = @STATUS           " +
                         "       ,PURCHASEDON = @PURCHASEDON " +
                         "       ,NOTE = @NOTE               " +
                         "       ,CATEGORY = @CATEGORY       " +
                         "  WHERE ID = @ID                   ";
            int row = await conn.ExecuteAsync(new CommandDefinition(sql, new
            {
                NAME = item.Name,
                STATUS = item.Status.ToString(),
                PURCHASEDON = item.PurchasedOn?.ToString("yyyy-MM-dd"),
                NOTE = item.Note,
                CATEGORY = item.Category,
                ID = item.Id
            }, cancellationToken : ct));
            return row == 1;
        }
    }
}
