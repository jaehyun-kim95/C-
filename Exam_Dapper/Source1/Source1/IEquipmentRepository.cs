namespace Source1
{
    internal interface IEquipmentRepository
    {
        Task<int> AddAsync(Equipment item, CancellationToken ct = default);
        Task<Equipment?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<Equipment>> SearchAsync(string? keyword, string? category, EquipmentStatus? status, CancellationToken ct = default);
        Task<PagedResult<Equipment>> GetPageAsync(int page, int pageSize, CancellationToken ct = default);
        Task<bool> UpdateAsync(Equipment item, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    }
}
