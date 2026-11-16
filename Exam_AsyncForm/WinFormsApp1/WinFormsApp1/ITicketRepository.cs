namespace WinFormsApp1
{
    internal interface ITicketRepository
    {
        Task<IReadOnlyList<Ticket>>GetTicketsAsync(string? keyword, CancellationToken ct);
        Task SaveAsync(IReadOnlyList<Ticket> tickets, CancellationToken ct);
    }
}
