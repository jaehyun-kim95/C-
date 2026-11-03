



async Task<string> CookAsync(string name, int sec)
{
    Console.WriteLine($"요리 시작 요리명 : {name}");
    await Task.Delay(sec * 1000);
    Console.WriteLine($"요리 완료 요리명 : {name}");
    return name;
}

//CookAsync("계란 후라이", 3);
//await CookAsync("라면", 2);
//await CookAsync("토스트", 1);
//Task<string> egg = CookAsync("계란 후라이", 3);
Task<string> noodle = CookAsync("라면", 2);
Task<string> toste = CookAsync("토스트", 1);

await Task.WhenAll(CookAsync("계란 후라이", 3), noodle, toste); 

