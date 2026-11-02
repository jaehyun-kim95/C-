using System.Diagnostics;

var books = new List<Book>
{
    new(1, "C# 입문",       "김철수",     "IT",   25000, 10),
    new(2, "LINQ 완전정복", "이영희",     "IT",   32000, 0),
    new(3, "클린 코드",     "로버트 마틴", "IT",   33000, 5),
    new(4, "어린 왕자",     "생텍쥐페리",  "소설",  9000, 20),
    new(5, "데미안",        "헤르만 헤세", "소설", 11000, 3),
    new(6, "총균쇠",        "재레드 다이아몬드", "역사", 28000, 7),
    new(7, "사피엔스",      "유발 하라리", "역사", 22000, 0),
    new(8, "요리의 기초",   "박요리",     "취미", 18000, 12),
};

var customers = new List<Customer>
{
    new(1, "홍길동", "서울", true),
    new(2, "마석도", "부산", false),
    new(3, "김민수", "서울", true),
    new(4, "이서연", "인천", false),
    new(5, "박지훈", "부산", true),
};

var orders = new List<Order>
{
    new(1, 1, 1, 2, new DateTime(2026, 10, 1)),
    new(2, 1, 3, 1, new DateTime(2026, 10, 3)),
    new(3, 2, 4, 3, new DateTime(2026, 10, 3)),
    new(4, 3, 1, 1, new DateTime(2026, 10, 5)),
    new(5, 3, 6, 2, new DateTime(2026, 10, 7)),
    new(6, 4, 5, 1, new DateTime(2026, 10, 8)),
    new(7, 2, 8, 2, new DateTime(2026, 10, 10)),
    new(8, 1, 4, 1, new DateTime(2026, 10, 12)),
};

void sj(IEnumerable<string> abc)
{
    Console.WriteLine(string.Join(",", abc));
}

//1
List<string> overBook = new List<string>();
foreach (var book in books) {
    if (book.Price >= 20000) {
        overBook.Add(book.Title);
    }
}
sj(overBook);
sj(books.Where(b => b.Price >= 20000).OrderBy(b => b.Price).Select(b => b.Title));
sj(books.Where(b=>b.Stock == 0).Select(b=>b.Title));
sj(books.Where(b => b.Category == "IT").Where(b => b.Stock > 0).Select(b => b.Title));

//2
sj(customers.Select(b=>b.Name));
sj(books.Select(b=>$"{b.Title} {b.Price * b.Stock}"));
sj(customers.Where(b=>b.IsMember == true).Select(b=>$"{b.Name}({b.City})"));

//3
sj(books.OrderBy(b=>b.Price).Select(b=>$"{b.Title} {b.Price}").Take(3));
sj(books.OrderBy(b=>b.Category).ThenByDescending(b=>b.Price).
    Select(b=>$"{b.Category} : {b.Title} {b.Price:N0}"));

var tt = books.OrderByDescending(b => b.Price).First();
var tt2 = books.MaxBy(b=>b.Price);
Console.WriteLine($"{tt.Title} {tt.Price}");
Console.WriteLine($"{tt2.Title} {tt2.Price}");

//4
Console.WriteLine(books.Any(b=>b.Stock == 0));
Console.WriteLine(books.All(b=>b.Price >= 5000));

foreach (var customer in customers)
{
    bool hasOrder = false;                

    foreach (var order in orders)
    {
        if (order.CustomerId == customer.Id)
        {
            hasOrder = true;
            break;                         
        }
    }

    if (!hasOrder)
    {
        Console.WriteLine(customer.Name);
    }
}

//not exists
sj(customers.Where(c => !orders.Any(o => o.CustomerId == c.Id))
            .Select(c=>c.Name));


//5
Console.WriteLine(books.Where(b => b.Stock > 0).Sum(b => b.Stock));
Console.WriteLine(books.Where(b => b.Category == "IT").Average(b => b.Price));
Console.WriteLine(books.Select(b=>b.Title)
       .Aggregate((acc, next) => acc + "/" + next)); 


//6
sj(books.GroupBy(b=>b.Category).Select(g=>$"{g.Key} {g.Count()}"));
sj(books.GroupBy(b=>b.Category).Select(g=>$"{g.Key} {g.Average(g=>g.Price)}"));
sj(orders.GroupBy(o=>o.CustomerId).Select(g=>$"고객 {g.Key}: {g.Sum(g=>g.Quantity)}건"));

//7 
sj(
    orders.Join(customers, o => o.CustomerId, c => c.Id, (o, c) => new { o.Id, c.Name, o.Quantity, o.BookId })
          .Join(books, j => j.BookId, b => b.Id, (j, b) => new { j.Id, j.Name, b.Title, j.Quantity })
          .Select(x => $"주문 {x.Id}: {x.Name} - {x.Title} x{x.Quantity}"));
sj(
orders.Join(books, o => o.BookId, b => b.Id, (o, b) => new { o.CustomerId, b.Price, o.Quantity })
      .Join(customers, j => j.CustomerId, c => c.Id, (j, c) => new { c.Name, j.Price, j.Quantity })
      .GroupBy(x => x.Name)
      .Select(g => new { Name = g.Key, Total = g.Sum(x => x.Price * x.Quantity) })      
      .OrderByDescending(t => t.Total)
      .Select(t => $"{t.Name} {t.Total:N0}"));


record Book(int Id, string Title, string Author, string Category, decimal Price, int Stock);
record Customer(int Id, string Name, string City, bool IsMember);
record Order(int Id, int CustomerId, int BookId, int Quantity, DateTime OrderDate);