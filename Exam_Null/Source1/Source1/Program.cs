void SendReminder(Person organizer, string roomName)
{
    if (organizer is null) {
        ArgumentNullException.ThrowIfNull(organizer);
    }
    Console.WriteLine($"  알림 → {organizer.Name} ({organizer.Email ?? "이메일 없음"}): {roomName} 예약 확인");
}

var rooms = new List<MeetingRoom>
{
    new MeetingRoom { Id = "R1", Name = "대회의실", Capacity = 20 },
    new MeetingRoom { Id = "R2", Name = "소회의실", Capacity = 6 },
};

var reservations = new List<Reservation>
{
    new Reservation { RoomId = "R1", Organizer = new Person { Name = "홍길동", Email = "hong@company.com" }, Memo = "주간 회의" },
    new Reservation { RoomId = "R2", Organizer = new Person { Name = "마석도" } },     // 이메일, 메모 없음
    new Reservation { RoomId = "R3", Organizer = null, Memo = "외부 손님" },           // 없는 회의실, 예약자 미지정
};

foreach (var r in reservations)
{
    MeetingRoom? room = FindRoom(r.RoomId);
    if (room is null) {
        Console.WriteLine($"[건너뜀] 존재하지 않는 회의실: {r.RoomId}");
        Console.WriteLine();
        continue;
    }
    Console.WriteLine($"회의실: {room.Name} (정원 {room.Capacity}명)");
    Console.WriteLine($"예약자: {r.Organizer?.Name ?? $"(미지정)"}");
    Console.WriteLine($"이메일 도메인: {r.Organizer?.Email?.Split('@')[1] ?? "(없음)"}");
    Console.WriteLine($"메모 길이: {r.Memo?.Length ?? 0}");
    if (r.Organizer is not null)
    {
        SendReminder(r.Organizer, room.Name);
    }
    Console.WriteLine();
}

MeetingRoom? FindRoom(string id)
{

    foreach (var room in rooms)
    {
        if (room.Id == id) return room;
    }
    return null;
}

class MeetingRoom
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public int Capacity { get; set; }
}

class Person
{
    public required string Name { get; set; }
    public string? Email { get; set; }
}

class Reservation
{
    public required string RoomId { get; set; }
    public Person? Organizer { get; set; }
    public string? Memo { get; set; }
}