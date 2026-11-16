namespace WinFormsApp1
{

    public enum TicketStatus { 접수 = 0, 처리중 = 1 , 완료 = 2};
    internal class Ticket
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public TicketStatus Status { get; set; }
        public string Assignee { get; set; } = "";
        public DateTime UpdatedAt { get; set; }
    }
}
