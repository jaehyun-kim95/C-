Console.WriteLine("단순 List 제너릭 활용");
var fruits = new List<string>();
fruits.Add("사과");
fruits.Add("배");

var score = new List<int>();
score.Add(90);
score.Add(85);

PrintList("과일", fruits);
PrintList("점수",score);

Console.WriteLine("-------------------");
Console.WriteLine("제너릭 class 활용.");

var empRepo = new Repository<Employee>();
empRepo.Add(new Employee("E001", "홍길동", "개발팀"));
empRepo.Add(new Employee("E002", "마석도", "영업팀"));
empRepo.Add(new Employee("E003", "김민수", "개발팀"));

var deviceRepo = new Repository<Device>();
deviceRepo.Add(new Device("D01", "정문", true));
deviceRepo.Add(new Device("D02", "후문", true));
deviceRepo.Add(new Device("D03", "창고", false));

var emp = empRepo.GetById("E002");
if (emp != null)
    Console.WriteLine($"E002 조회: {emp.Name} ({emp.Department})");
else
    Console.WriteLine("E002 조회: 없음");

var notFound = empRepo.GetById("E999");
Console.WriteLine($"E999 조회: {notFound?.Name ?? "없음"}");

var device = deviceRepo.GetById("D03");
if (device != null)
{
    string status = device.IsActive ? "사용중" : "중지";
    Console.WriteLine($"D03 조회: {device.Location} ({status})");
}

Console.WriteLine($"직원 {empRepo.Count}명, 장비 {deviceRepo.Count}대");

try
{
    empRepo.Add(new Employee("E001", "홍길동2", "기획팀"));
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"오류: {ex.Message}");
}

// 삭제
Console.WriteLine($"E003 삭제: {empRepo.Remove("E003")}");   // True
Console.WriteLine($"E999 삭제: {empRepo.Remove("E999")}");   // False
Console.WriteLine($"남은 직원 {empRepo.Count}명");            // 2명

// 전체 출력 (1단계 PrintList 재사용)
PrintList("직원 목록", empRepo.GetAll());
PrintEntities("직원 목록", empRepo.GetAll());

void PrintEntities<T> (string title, IEnumerable<T> items) where T : IEntity
{
    Console.WriteLine($"[{title}]");
    foreach (T item in items)
    {        
        Console.WriteLine($"[{item.Id}] {item}");
    }
}

void PrintList<T>(string title, List<T> items)
{
    Console.WriteLine($"[{title}]");
    for (int i = 0; i < items.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {items[i]}");
    }
}

class Repository<T> where T : class, IEntity
{
    private Dictionary<string,T> items;
    public int Count => items.Count;

    public Repository()
    {
        this.items = new Dictionary<string, T>();
    }

    public void Add(T item)
    {
        if (items.ContainsKey(item.Id)) {
            throw new InvalidOperationException($"이미 존재하는 id입니다: {item.Id}");
        }
        else
        {
            items.Add(item.Id, item);
        }        
    }

    public bool Remove(string id) {
        return items.Remove(id);
    }

    public List<T> GetAll() { return new List<T>(items.Values); }

    public T? GetById(string id)
    {
        items.TryGetValue(id, out var item);
        return item;
    }
    
}

interface IEntity
{
    string Id { get; }
}


record Employee(string Id, string Name, string Department) : IEntity;
record Device(string Id, string Location, bool IsActive) : IEntity;
