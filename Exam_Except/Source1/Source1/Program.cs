
using System.Diagnostics;

string path = "C:\\Code Study\\C#\\Exam_Except\\Source1\\Source1\\access.log";
string[] text;
int lineNum = 1;

// 1. 파일이 없을 때 처리
try {
    text = await File.ReadAllLinesAsync(path);
}
catch(FileNotFoundException ex)
{
    Console.WriteLine(ex.Message);
    return;
}
catch (DirectoryNotFoundException ex)
{
    Console.WriteLine(ex.Message);
    return;
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    return;
}


var logs = new List<AccessLog>();
var errors = new List<string>();

using (new OperationTimer("리포트 생성"))
{
    try
    {
        using var writer = new StreamWriter("report.txt");
        foreach (string line in text)
        {
            try
            {
                AccessLog log = ParseLIne(line, lineNum);
                logs.Add(log);
            }
            catch (InvalidLogLineException ex)
            {
                errors.Add($"[{ex.LineNumer}번째 줄] {ex.Message} => 원문 : {ex.RawLine}");
                writer.WriteLine($"[{ex.LineNumer}번째 줄] {ex.Message} => 원문 : {ex.RawLine}");
            }
            lineNum++;
        }

        Console.WriteLine($"정상 {logs.Count}건, 오류 {errors.Count}건");
        foreach (string err in errors) { Console.WriteLine($"  {err}"); }
    }
    catch (DirectoryNotFoundException)
    {
        Console.WriteLine("리포트를 저장할 폴더가 없습니다.");
    }
    catch (IOException ex)
    {
        Console.WriteLine($"리포트 저장 중 오류: {ex.Message}");
    }
}

AccessLog ParseLIne(string line, int lineNumber) {
    string[] parts = line.Split(',');
    if (parts.Length != 5)
    {
        throw new InvalidLogLineException("형식 개수 오류",lineNumber,line);
    }

    if (!DateTime.TryParse(parts[0], out DateTime dt))
    {
        throw new InvalidLogLineException("날짜 형식 오류", lineNumber, line);
    }

    return new AccessLog(dt, parts[1], parts[2], parts[3], parts[4]);
}


class OperationTimer : IDisposable
{
    private string _name;
    private Stopwatch _sw;


    public OperationTimer(string name)
    {
        this._name = name;
        Console.WriteLine($"[시작] {_name}");
        _sw = Stopwatch.StartNew();
    }

    public void Dispose()
    {
        _sw.Stop();
        Console.WriteLine($"[종료] {_sw.ElapsedMilliseconds}");
    }
}



class InvalidLogLineException : Exception
{
    public int LineNumer { get; }
    public String? RawLine { get; }

    public InvalidLogLineException(string? message, int lineNumber, string rawLine) : base(message)
    {
        this.LineNumer = lineNumber;
        this.RawLine = rawLine;
    }
}


record AccessLog(DateTime Time, string Id, string Name, string Gate, string Type);