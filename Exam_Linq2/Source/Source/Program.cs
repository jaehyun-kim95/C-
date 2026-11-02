
var parcels = CreateParcels();
int checks = 0;

var heavy = parcels.Where(p => { checks++; return p.WeightKg >= 10; }).ToList();
Console.WriteLine($"쿼리 생성 후: 검사 {checks}회");

int count = heavy.Count();
Console.WriteLine($"Count() 후: 검사 {checks}회 (결과 {count}건)");

foreach (var p in heavy) { }
Console.WriteLine($"foreach 후: 검사 {checks}회");

bool any = heavy.Any();
Console.WriteLine($"Any() 후: 검사 {checks}회");



// ===== 데이터 =====
List<Parcel> CreateParcels() => new()
{
    new("P001", "서울",  3, "배송중"),
    new("P002", "부산", 12, "배송중"),
    new("P003", "서울",  7, "배송완료"),
    new("P004", "대구",  1, "배송중"),
    new("P005", "서울", 15, "배송중"),
};

class Parcel
{
    public string TrackingNo { get; }
    public string Region { get; }
    public int WeightKg { get; }
    public string Status { get; set; }    // 상태는 바뀔 수 있음

    public Parcel(string trackingNo, string region, int weightKg, string status)
    {
        TrackingNo = trackingNo;
        Region = region;
        WeightKg = weightKg;
        Status = status;
    }
}