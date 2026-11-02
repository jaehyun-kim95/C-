

var fullEmpl = new TFullTimeEmployee(1, "홍길동", new DateTime(2020, 08, 01), 350000);
var partTimeEmpl = new TPartTimeEmployee(2, "마석도", new DateTime(2019, 08, 01), 12000);
partTimeEmpl.AddHours(2);
partTimeEmpl.AddHours(2);

decimal total = 0;
var emps = new List<TEmployee> { fullEmpl, partTimeEmpl };
foreach (var e in emps)
{
    Console.WriteLine(e.GetPayInfo());
    total += e.CalculatePay();
}

Console.WriteLine($"지불할 금액 : {total:N0}");

try
{
    fullEmpl.Name = "";
    Console.WriteLine(fullEmpl.Name);
}
catch (ArgumentException Ex)
{
    Console.WriteLine(Ex.Message);
}




interface IPayable 
{
    string GetPayInfo();
    decimal CalculatePay();
}

abstract class TEmployee : IPayable 
{
    public TEmployee(int FId, string FName, DateTime FHire)
    {
        this._id = FId;
        this.Name = FName;
        this._hire = FHire;
    }

    private int _id;
    private string _name;

    private DateTime _hire;    

    public int FId { get => _id; }
    
    public string Name {
        get => _name;
        set {
            if (string.IsNullOrWhiteSpace(value)) {
                throw new ArgumentException("이름은 비어 있을 수가 없습니다.");
            } else
            {
                this._name = value.Trim();
            }
        } 
    }

    public DateTime Hire { get => _hire; }

    virtual public string GetPayInfo()
    {       
         return $"[ID : {_id}] {_name}, " +
                $"고용일자 : {_hire.ToString("yyyy-MM-dd")} (근속 {YearsOfService()}년)";
    }

    public int YearsOfService()
    {
        var today = DateTime.Today;
        int years = today.Year - _hire.Year;
        if (_hire.Date > today.AddYears(-years)) years--;   
        return years;
    }

    abstract public decimal CalculatePay();

}

// 정규직
class TFullTimeEmployee : TEmployee
{
    private decimal ASalary { get; }
    public TFullTimeEmployee(int FId, string FName, DateTime FHire, decimal FSale) : base(FId, FName, FHire)
    {

        if (FSale < 0)
        {
            throw new ArgumentException("월급은 음수를 넣을 수 없습니다.");
        }
        else
        {
            this.ASalary = FSale;
        }
    }

    public override string GetPayInfo()
    {
        return base.GetPayInfo() + $" 월급 : {this.ASalary}";
    }

    public override decimal CalculatePay()
    {
        return this.ASalary + YearsOfService() * 50000;
    }


}

// 알바
class TPartTimeEmployee : TEmployee
{
    private decimal ARate { get; }
    private decimal AHours { get; set; }

    public TPartTimeEmployee(int FId, string FName, DateTime FHire, decimal FRate) : base(FId, FName, FHire)
    {
        this.ARate = FRate;
    }

    public void AddHours(decimal AHour) {
        if (AHour <= 0)
        {
            throw new ArgumentException("근무 시간은 0시간 이하로 입력할 수 없습니다.");
        }
        else
        {
            this.AHours += AHour;
        }
    }

    public override string GetPayInfo()
    {
        return base.GetPayInfo() + $" 시급 : {this.ARate}";
    }

    public override decimal CalculatePay()
    {
        return this.ARate * this.AHours;
    }
}
