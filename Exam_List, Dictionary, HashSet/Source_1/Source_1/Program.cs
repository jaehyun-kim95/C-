
string[] texts = await File.ReadAllLinesAsync("C:\\Code Study\\C#\\Exam_List, Dictionary, HashSet\\Source_1\\Source_1\\access.log");
var logs = new List<AccessLog>(); // 정상적인 줄.
var errors = new List<string>(); // 오류 줄.

foreach (string text in texts) {
    // 1. 정상적인 줄, 오류 줄 구별
    string[] parts = text.Split(',');
    if (parts.Length != 5)
    {
        errors.Add(text);
        continue;
    }


    if (!DateTime.TryParse(parts[0], out DateTime dt))
    {
        errors.Add(text);
        continue;
    }

    logs.Add(new AccessLog(dt, parts[1], parts[2], parts[3], parts[4]));
}
Console.WriteLine($"정상적인 줄 : {logs.Count}, 오류 줄 : {errors.Count}");

// 2. id & 사람 별 출입 횟수
var counts = new Dictionary<string, int>();
var names = new Dictionary<string, string>();

foreach (AccessLog log in logs) { 
    if (counts.ContainsKey(log.Id)) { 
        counts[log.Id]++;   
    }else
    {
        counts.Add(log.Id, 1);
    }
    names[log.Id] = log.Name;
}

// 2.1 정렬
var ids = new List<string>(counts.Keys); // key 로 list를 구성.
ids.Sort(); // key 정렬.

foreach (string id in ids) { 
    Console.WriteLine($"{id} {names[id]} : {counts[id]}"); // 정렬된 key로 출력.
}


// 3. 출입구 별 출입 횟수
var gateCounts = new Dictionary<string, int>(); 
var gateMembers = new Dictionary<string, HashSet<string>>(); // key인 Gate를 이용한 사람의 집합.

foreach(AccessLog log in logs){
    if (gateCounts.ContainsKey(log.Gate))
    {
        gateCounts[log.Gate]++;
    }else
    {
        gateCounts.Add(log.Gate,1);
    }

    if (!gateMembers.ContainsKey(log.Gate)) {
        gateMembers.Add(log.Gate,new HashSet<string>());
    }
    gateMembers[log.Gate].Add(log.Id);
}

var gates = new List<String>(gateCounts.Keys);
gates.Sort();

foreach (string gate in gates)
{
    var memberIds = new List<string>(gateMembers[gate]);
    memberIds.Sort();

    Console.WriteLine($"{gate} : {gateCounts[gate]} / {memberIds.Count}명 {string.Join(",",memberIds)}");
}

// 4. 이틀 출근한 직원.
var dayMembers = new Dictionary<DateTime,HashSet<String>>();
foreach (AccessLog log in logs) {
    DateTime day = log.Time.Date;

    if (!dayMembers.ContainsKey(day)) {
        dayMembers.Add(day, new HashSet<string>());
    }
    dayMembers[day].Add(log.Id);
}

// 4-1. 비교
var day1 = dayMembers[new DateTime(2026, 10, 12)];
var day2 = dayMembers[new DateTime(2026, 10, 13)];

var both = new HashSet<string>(day1); // 교집합.
both.IntersectWith(day2);

var onlyday1 = new HashSet<string>(day1); // 차집합.
onlyday1.ExceptWith(day2);

var onlyday2 = new HashSet<string>(day2); // 차집합.
onlyday2.ExceptWith(day1);

Console.WriteLine();
PrintNames("이틀 모두 출근", both);
PrintNames("10-12에만 출근", onlyday1);
PrintNames("10-13에만 출근", onlyday2);


// 사번 집합을 정렬해서 이름으로 출력하는 로컬 함수
void PrintNames(string title, HashSet<string> idSet)
{
    var sorted = new List<string>(idSet);
    sorted.Sort();

    var nameList = new List<string>();
    foreach (string id in sorted)
    {
        nameList.Add(names[id]);
    }  

    Console.WriteLine($"{title}: {string.Join(", ", nameList)}");
}


record AccessLog(DateTime Time, string Id, string Name, string Gate, string Type);