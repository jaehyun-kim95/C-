
namespace WinFormsApp1
{
    internal class FakeTicketRepository(bool _random) : ITicketRepository
    {
        public int Latency { get; set; } = 3000;
        bool RandomLatency = _random;

        private readonly List<Ticket> _ticketList = new()
        {
            new Ticket { Id = "101", Title = "test1", Status = TicketStatus.처리중, Assignee = "김민수", UpdatedAt = DateTime.Parse("2026-11-12") },
            new Ticket { Id = "102", Title = "test2", Status = TicketStatus.접수, Assignee = "박지영", UpdatedAt = DateTime.Parse("2026-11-13") },
            new Ticket { Id = "103", Title = "test3", Status = TicketStatus.완료, Assignee = "이서준", UpdatedAt = DateTime.Parse("2026-11-10") },
            new Ticket { Id = "104", Title = "test4", Status = TicketStatus.처리중, Assignee = "김민수", UpdatedAt = DateTime.Parse("2026-11-14") },
            new Ticket { Id = "105", Title = "test5", Status = TicketStatus.접수, Assignee = "최유진", UpdatedAt = DateTime.Parse("2026-11-15") },
            new Ticket { Id = "106", Title = "test6", Status = TicketStatus.완료, Assignee = "홍길동", UpdatedAt = DateTime.Parse("2026-11-05") }
        };


        public async Task<IReadOnlyList<Ticket>> GetTicketsAsync(string? keyword, CancellationToken ct)
        {
            IEnumerable<Ticket> query = _ticketList;
            if(RandomLatency)
            {
                await Task.Delay(Random.Shared.Next(1000, 4001), ct);
            } else
            {
                await Task.Delay(Latency, ct);
            }           

            if (!String.IsNullOrWhiteSpace(keyword)) {
                query = query.Where(w => w.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            return query.Select(s => new Ticket { Id = s.Id, Title = s.Title, Status = s.Status, Assignee = s.Assignee, UpdatedAt = s.UpdatedAt}).ToList();
        }

        public async Task SaveAsync(IReadOnlyList<Ticket> tickets, CancellationToken ct)
        {
            if (RandomLatency)
            {
                await Task.Delay(Random.Shared.Next(1000, 4001), ct);
            }
            else
            {
                await Task.Delay(Latency, ct);
            }


        }
    }
}
