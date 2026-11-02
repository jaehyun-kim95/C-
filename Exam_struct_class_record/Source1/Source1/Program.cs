void Check(string label, bool ok)
{
    Console.WriteLine($"{(ok ? "통과" : "실패")} : {label}");
}

PositionS s1 = new PositionS(1,1); //변수 타입.
var s2 = s1;
s2.X = 99;
Check("struct : 복사본을 바꾼 뒤 원본 X",s1.X == 1);

PositionC c1 = new PositionC(1,1); // 참조타입.
var c2 = c1;
c2.X = 99;
Check("class : 복사본을 바꾼 뒤 원본 X",c1.X == 99);

PositionR r1 = new PositionR(1, 1); // record 데이터 변경 불가.
var r2 = r1; // 참조타입 - 주소 값 복사.
var r3 = r1 with { X = 99 }; // with로 새로운 객체 전달.
Check("record: with 후 원본 X", r1.X == 1);
Check("record: r1과 r2는 같은 객체", ReferenceEquals(r1, r2) == true);
Check("record: r1과 r3는 같은 객체", ReferenceEquals(r1, r3) == false);

void MoveS(PositionS p) { p.X += 10; }
void MoveRefS(ref PositionS p) { p.X += 10; }
void MoveC(PositionC p) { p.X += 10; }
void ReplaceC(PositionC p) { p = new PositionC(0, 0); }

var s = new PositionS(1, 1);
MoveS(s);
Check("struct를 그냥 넘김", s.X == 1);
MoveRefS(ref s);
Check("struct를 ref로 넘김", s.X == 11);

var c = new PositionC(1, 1);
MoveC(c);
Check("class를 넘겨서 값 변경", c.X == 11);
ReplaceC(c);
Check("class를 넘겨서 새 객체로 교체", c.X == 11);


var sa = new PositionS(1, 1);
var sb = new PositionS(1, 1);
Check("struct Equals (같은 값)", sa.Equals(sb) == true);
// Check("struct ==", sa == sb);    // 주석을 풀어 보세요

var ca = new PositionC(1, 1);
var cb = new PositionC(1, 1);
Check("class Equals (같은 값, 다른 객체)", ca.Equals(cb) == false);
Check("class ==", (ca == cb) == false);

var ra = new PositionR(1, 1);
var rb = new PositionR(1, 1);
Check("record Equals (같은 값, 다른 객체)", ra.Equals(rb) == true);
Check("record ==", (ra == rb) == true);

// struct 목록
var listS = new List<PositionS> { new(1, 1) };
//listS[0].X = 5;                       // ① 주석을 풀어 보세요

var tmp = listS[0];                      // ② 꺼내서
tmp.X = 5;                               //    바꾸고
listS[0] = tmp;                          //    다시 넣기
Check("List<struct> 꺼내서 바꾸고 다시 넣기", listS[0].X == 5);

// struct 배열
var arrS = new PositionS[] { new(1, 1) };
arrS[0].X = 5;                           // ③ 배열은 됨
Check("struct 배열 직접 수정", arrS[0].X == 5);

// class 목록
var listC = new List<PositionC> { new(1, 1) };
listC[0].X = 5;
Check("List<class> 직접 수정", listC[0].X == 5);

// foreach 안에서 수정
// foreach (var p in listS) p.X = 0;    // ④ 주석을 풀어 보세요

// 기본 타입
int n = 10;
object o = n;          // 박싱: 10을 상자에 복사
n = 20;
Check("박싱 후 원본을 바꾸면 상자 안의 값", (int)o == 10);   // 언박싱

// 언박싱은 정확한 타입으로만
try
{
    long l = (long)o;   // 상자 안은 int인데 long으로 꺼내려 함
    Check("int 상자를 long으로 언박싱", false);
}
catch (InvalidCastException)
{
    Check("int 상자를 long으로 언박싱하면 예외", true);
}
long ok = (long)(int)o;   // int로 꺼낸 뒤 long으로 변환하면 됨

// struct 박싱
var sp = new PositionS(1, 1);
object boxed = sp;
sp.X = 99;
Check("struct 박싱 후 원본 변경 → 상자 안 X", ((PositionS)boxed).X == 1);

// 인터페이스로 다룰 때도 박싱 (가장 모르고 지나가기 쉬움)
var sm = new PositionS(1, 1);
IMovable m = sm;        // 박싱
m.Move();               // 상자 안의 복사본이 움직임
Check("인터페이스로 Move 후 원본 X", sm.X == 1);
Check("인터페이스로 Move 후 상자 안 X", ((PositionS)m).X == 11);

// class는 박싱이 없음
var cm = new PositionC(1, 1);
IMovable mc = cm;
mc.Move();
Check("class를 인터페이스로 Move 후 원본 X", cm.X == 11);



interface IMovable
{
    void Move();
}


struct PositionS : IMovable
{
    public int X { get; set; }
    public int Y { get; set; }
    public PositionS(int x, int y ) {
        this.X = x; 
        this.Y = y; 
    }
    public void Move() {
        X += 10;
    }
}

class PositionC : IMovable
{
    public int X { get; set; }
    public int Y { get; set; }
    public PositionC(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
    public void Move()
    {
        X += 10;
    }
}

record PositionR(int X, int Y);



