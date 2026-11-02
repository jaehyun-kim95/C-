
using System.Numerics;

var empRepo = new Repository<Employee>();
empRepo.Add(new Employee("E001", "홍길동", "개발팀"));
empRepo.Add(new Employee("E002", "마석도", "영업팀"));
empRepo.Add(new Employee("E003", "김민수", "개발팀"));

var deviceRepo = new Repository<Device>();
deviceRepo.Add(new Device("D01", "정문", true));
deviceRepo.Add(new Device("D02", "후문", true));
deviceRepo.Add(new Device("D03", "창고", false));

void PrintUpper(string message)
{
    Console.WriteLine($"[알림] {message}");
}


void PrintNormal(string message)
{
    Console.WriteLine(message);
}

void PrintLog(string name) {
    Console.WriteLine($"[로그] {name}출입");
}

void PrintLength(string word) {
    Console.WriteLine($"글자 수: {word.Length}");
}

int Calculate(int a, int b, Func<int, int, int> operation) {
    return operation(a, b);
}

Func<int, int, int> sum = (a, b) => a + b;
Func<int, int, int> multiply = (a, b) => a + b;
Action<string> handler = PrintNormal;
Action<string> handler1 = PrintUpper;

handler("홍길동 출입");
handler1("홍길동 출입");

List<Employee> devTem = empRepo.Find(emp => emp.Department == "개발팀");

var devName = new List<string>();
foreach (Employee devNames in devTem)
{
    devName.Add(devNames.Name);
}
Console.WriteLine(string.Join(",",  devName));

Action<string> funs = null;
funs += PrintUpper;
funs += PrintLog;
funs += PrintLength;

funs.Invoke("홍길동");


class Repository<T> where T : class, IEntity
{
    private Dictionary<string, T> items;
    public int Count => items.Count;

    public Repository()
    {
        this.items = new Dictionary<string, T>();
    }

    public void Add(T item)
    {
        if (items.ContainsKey(item.Id))
        {
            throw new InvalidOperationException($"이미 존재하는 id입니다: {item.Id}");
        }
        else
        {
            items.Add(item.Id, item);
        }
    }

    public bool Remove(string id)
    {
        return items.Remove(id);
    }

    public List<T> GetAll() { return new List<T>(items.Values); }

    public T? GetById(string id)
    {
        items.TryGetValue(id, out var item);
        return item;
    }

    public List<T> Find(Func<T, bool> condition)
    {
        var result = new List<T>();
        foreach (T item in items.Values)
        {
            if (condition(item))
            {
                result.Add(item);
            }
        }
        return result;
    }
    

}

interface IEntity
{
    string Id { get; }
}


record Employee(string Id, string Name, string Department) : IEntity;
record Device(string Id, string Location, bool IsActive) : IEntity;


