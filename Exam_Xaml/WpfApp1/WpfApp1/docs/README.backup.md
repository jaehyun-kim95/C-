# WPF 학습 노트: XAML 레이아웃, 데이터 바인딩, Command

실습 화면: 직원 정보 등록 (헤더 / 왼쪽 메뉴 / 입력 폼 / 버튼바 / 상태바)

---

## 1. 레이아웃

### Grid 크기 단위

| 값 | 의미 | 쓰는 곳 |
|---|---|---|
| `50` | 고정 크기 | 헤더 높이, 메뉴 너비, 라벨 열 |
| `Auto` | 내용 크기만큼 | 버튼바, 상태바, 폼의 입력 행 |
| `*` / `2*` | 남은 공간 전부 / 비율 | 본문, 메모처럼 늘어나야 하는 칸 |

- `Auto` 행에 빈 `Border`만 있으면 높이가 0이 되어 사라진다. 내용이 있으면 그 높이만큼.
- 창을 늘리면 `*`인 행/열만 늘어난다.

### Grid vs StackPanel

| | Grid | StackPanel |
|---|---|---|
| 배치 | 행/열 표 | 한 줄로 쌓기 (`Orientation`) |
| 남은 공간 채우기 | 가능 (`*`) | 불가 |
| 용도 | 화면 뼈대, 라벨+입력 정렬, 양쪽 끝 정렬 | 버튼 묶음, 메뉴 목록 |

- WPF `StackPanel`에는 간격 속성이 없다. 자식마다 `Margin`으로 준다.
- 양쪽 끝 정렬은 2열 Grid(`*` / `Auto`)가 기본. 같은 칸에 겹쳐 놓고 `Left`/`Right`로 정렬하면 창이 좁아질 때 겹친다.

### Delphi 대응

| Delphi | WPF |
|---|---|
| `Align := alTop` | Grid 행 `Height="50"` 또는 `Auto` |
| `Align := alClient` | Grid 행/열 `*` |
| `Align := alLeft` + `Width` | Grid 열 `Width="180"` |
| `Anchors := [akLeft, akRight]` | `HorizontalAlignment="Stretch"` (기본값) |
| `TPanel`에 버튼 나열 | `StackPanel Orientation="Horizontal"` |

### 꼭 기억할 규칙

- **`Window`와 `Border`는 자식을 하나만** 가진다. 여러 개면 Grid/StackPanel로 감싼다.
- **`Grid.Row` / `Grid.Column`은 Grid의 직계 자식에만 효과가 있다.** 부모가 Border나 ScrollViewer면 무시된다 (오류 없음).
- 같은 칸에 자식이 여러 개면 겹쳐 그려진다. `Grid.Row`를 빼먹으면 0행으로 가서 겹친다.
- 디자이너로 끌어다 놓지 않는다. `Margin`에 좌표가 박혀 크기 조절 시 깨진다.

### 여백과 정렬

| 속성 | 의미 |
|---|---|
| `Margin` | 바깥 여백 (`"좌,위,우,아래"`) |
| `Padding` | 안쪽 여백 |
| `HorizontalAlignment` / `VerticalAlignment` | 칸 안에서 컨트롤의 위치 |
| `HorizontalContentAlignment` / `VerticalContentAlignment` | 컨트롤 안에서 내용의 위치 |

**입력칸 폭 고정하기**

| 조합 | 결과 |
|---|---|
| `Left` + `Width=220` | 왼쪽에 220. 창이 좁아지면 잘림 → **창 `MinWidth`로 막는다 (권장)** |
| `Left` + `MaxWidth=220` | 내용 너비만큼만 그려짐 (빈 칸이면 아주 좁음) |
| `Stretch` + `MaxWidth=220` | 220까지 늘고 남는 공간에서 가운데 정렬 |

### ScrollViewer

- 세로 스크롤바 기본값이 **`Visible`(항상 표시)**이다. `VerticalScrollBarVisibility="Auto"`를 준다.
- 높이가 정해진 곳(`*` 행 안)에 있어야 스크롤된다. StackPanel 안에 넣으면 높이 제한이 없어 스크롤되지 않는다.
- 배경/구분선이 있는 영역은 `Border > ScrollViewer > StackPanel` 순서로 감싼다. 반대면 스크롤바가 구분선 밖에 생긴다.

### StatusBar

- 안쪽이 `DockPanel`이라 **마지막 아이템이 남은 공간을 차지**한다. 마지막 아이템에 `HorizontalAlignment="Right"`를 주면 오른쪽 끝에 붙는다.
- 아이템이 3개 이상이면 가운데 것은 오른쪽으로 보낼 수 없다. 그때는 안에 Grid를 넣는다.

### Style

```xml
<Window.Resources>
    <Style x:Key="MenuButton" TargetType="Button">
        <Setter Property="Padding" Value="8"/>
    </Style>
    <Style x:Key="SelectedMenuButton" TargetType="Button" BasedOn="{StaticResource MenuButton}">
        <Setter Property="FontWeight" Value="Bold"/>
    </Style>
</Window.Resources>

<Button Style="{StaticResource MenuButton}"/>
```

- **우선순위: 컨트롤에 직접 쓴 값 > Style Setter.** 하나만 다르게 하려면 그 컨트롤에 직접 쓴다.
- `BasedOn`으로 물려받고 다른 부분만 덮어쓴다.
- **`x:Key` 없는 Style(암시적)**은 범위 안의 **모든** 해당 타입 컨트롤에 적용된다. 앱 전체 통일에는 좋지만, 의도치 않은 컨트롤(저장/취소 버튼 등)에도 퍼진다. `Grid.Resources`처럼 좁은 범위에 두면 안전하다.
- 타입이 다른 컨트롤(TextBox, ComboBox, DatePicker)의 공통 속성은 `TargetType="Control"` Style을 만들고 각 Style이 `BasedOn`한다.
- 기본 Button은 마우스를 올리면 템플릿의 hover 색이 배경을 덮는다. 막으려면 `ControlTemplate`을 바꿔야 한다.

---

## 2. 데이터 바인딩

### 구성

| Delphi | WPF |
|---|---|
| `TDBEdit.DataSource` + `DataField` | `Text="{Binding Name}"` |
| `TDataSource.DataSet` | `DataContext` |
| 필드 변경 → DB 컨트롤 갱신 | 속성 변경 → `PropertyChanged` → 화면 갱신 |

```csharp
public partial class MainWindow : Window
{
    private readonly EmployeeViewModel _vm = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _vm;          // 이 순간 바인딩이 값을 처음 읽는다
    }
}
```

- **`DataContext`는 자식에게 상속된다.** Window에 한 번 지정하면 깊이 있는 컨트롤도 같은 객체를 본다.
- ViewModel은 별도 파일, `public class`. 컨트롤을 전혀 모른다.
- 코드비하인드에서 ViewModel이 필요하면 `DataContext as ...` 대신 **필드로 보관**한다. (`as`는 실패를 null로 숨겨 엉뚱한 곳에서 NullReferenceException이 난다. 형변환이 필요하면 `(T)obj`로 바로 실패하게.)

### Binding 문법

```xml
Text="{Binding Name}"                                          <!-- Path= 생략 가능 -->
Text="{Binding Name, UpdateSourceTrigger=PropertyChanged}"     <!-- 옵션은 쉼표로 -->
Text="{Binding Email, Mode=OneWay}"
```

### Mode (방향)

| Mode | 방향 | 기본값인 곳 |
|---|---|---|
| `OneWay` | ViewModel → 화면 | `TextBlock.Text`, `IsEnabled`, `Content` 등 대부분 |
| `TwoWay` | 양방향 | `TextBox.Text`, `ComboBox.SelectedItem`, `DatePicker.SelectedDate` |
| `OneTime` | 처음 한 번만, 이후 알림 무시 | 없음. 바뀌지 않는 값(코드 목록, 로그인 사용자)에 |

### UpdateSourceTrigger (화면 → ViewModel 시점)

- **`TextBox.Text`의 기본값은 `LostFocus`.** 포커스가 빠질 때 값이 넘어간다.
- 버튼 클릭 시 포커스가 버튼으로 가서 값이 넘어가지만, **포커스를 받지 않는 버튼(`Focusable=False`, `ToolBar` 안 버튼)은 마지막 입력값이 빠진 채 실행된다.**
- `PropertyChanged`는 글자마다 전달. 검증/계산/조회가 붙은 칸은 무거워지고 미완성 값으로 오류가 날 수 있으니 필요한 칸에만.
- `ComboBox.SelectedItem`, `DatePicker.SelectedDate`는 선택 즉시 전달된다.

---

## 3. INotifyPropertyChanged

ViewModel이 "이 속성 바뀌었음"을 알려야 화면이 다시 읽는다. 알리지 않으면 **ViewModel과 화면이 서로 다른 값**을 갖게 되고, 저장하면 화면에 안 보이는 값이 저장된다.

### 공통 부모 클래스

```csharp
public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;   // 같으면 끝냄
        field = value;
        OnPropertyChanged(propertyName);
        return true;                                                          // 실제로 바뀜
    }
}
```

| 요소 | 의미 |
|---|---|
| `[CallerMemberName]` | 인자를 생략하면 호출한 멤버 이름이 자동으로 들어감 |
| `ref T field` | 필드 자체를 넘겨 메서드 안에서 대입 (Delphi `var`). **속성은 넘길 수 없다** (CS0206) |
| `EqualityComparer<T>.Default` | 제네릭에서 `==` 대신 쓰는 비교 |
| `bool` 반환 | 실제로 바뀌었을 때만 추가 작업(연관 알림 등) |

**"같으면 끝냄"이 필요한 이유**: 불필요한 다시 그리기 방지, A↔B 상호 갱신의 무한 반복 방지, TwoWay 왕복이 한 번에 멈추게 함.

### 일반 속성

```csharp
private string _email = "";
public string Email
{
    get => _email;
    set => SetProperty(ref _email, value);
}
```

### 계산 속성과 연관 알림

```csharp
public string Summary => $"편집 중: {Name} ({EmployeeNo}) 부서: {Department}";
public bool CanSave   => !string.IsNullOrWhiteSpace(Name);

public string Name
{
    get => _name;
    set
    {
        if (SetProperty(ref _name, value))
        {
            OnPropertyChanged(nameof(Summary));
            OnPropertyChanged(nameof(CanSave));
        }
    }
}
```

- 계산 속성은 **필드 없이 getter에서 매번 계산**한다. 생성자에서 한 번 만들어 저장하면 알림이 가도 옛 값이 나온다. (Delphi 계산 필드와 같은 개념)
- 바인딩은 getter 안을 볼 수 없어 의존 관계를 모른다. **원본 속성의 setter가 대신 알린다.**
- 계산 속성이 늘면 원본 setter마다 알림이 늘고 빠뜨리기 쉽다. CommunityToolkit.Mvvm은 `[NotifyPropertyChangedFor]`로 해결한다.
- 읽기 전용 속성을 `TextBox.Text`에 바인딩하면 기본 TwoWay라 예외가 난다. `Mode=OneWay`를 명시한다.

---

## 4. 컨트롤별 바인딩

### ComboBox

```xml
<ComboBox ItemsSource="{Binding Departments}" SelectedItem="{Binding Department}"/>
```

```csharp
public IReadOnlyList<string> Departments { get; } = new[] { "개발1팀", "개발2팀", "인사팀" };
```

- 목록은 XAML이 아니라 **ViewModel에** 둔다. 나중에 Repository에서 받아 채운다.
- XAML에 `ComboBoxItem`을 둔 채 `SelectedItem`을 string에 바인딩하면, `SelectedItem`에는 `ComboBoxItem` 객체가 들어 있어 서로 맞지 않는다.
- **목록에 없는 값**을 `SelectedItem`에 넣으면 화면은 이전 선택 그대로, ViewModel에는 그 값이 들어간다 (오류 없음). 값이 목록 안에 있는지는 ViewModel이 책임진다.

### DatePicker

```xml
<DatePicker SelectedDate="{Binding HireDate}"/>
```

```csharp
private DateTime? _hireDate = new DateTime(2026, 3, 2);
```

- 바인딩 대상은 `Text`가 아니라 **`SelectedDate`**.
- 타입은 **`DateTime?`**. 날짜가 없는 상태(신규 등록)를 `null`로 표현한다. `DateTime`이면 `0001-01-01`이 들어간다.
- 날짜는 `DateTime.Parse("...")` 대신 `new DateTime(y, m, d)`. 문자열 해석은 PC 지역 설정에 영향을 받는다.
- 출력은 `$"{HireDate:yyyy-MM-dd}"`. null이면 빈 문자열.

---

## 5. 바인딩이 안 될 때 점검 순서

| 증상 | 원인 | 드러나는 방식 |
|---|---|---|
| 화면 전체가 빈칸 | `DataContext` 미지정 (null) | **출력 창에도 오류 없음** |
| 한 칸만 빈칸 | 속성 이름 오타, 대소문자 차이, 필드만 있고 속성 없음 | 출력 창 `BindingExpression path error` |
| ViewModel 값을 바꿔도 화면 그대로 | `INotifyPropertyChanged` 미구현, setter에서 알림 누락 | 오류 없음 |
| 알림을 보내는데도 화면 그대로 | 알림 이름이 틀림 (예: `nameof(propertyName)` → `"propertyName"`) | **오류 없음** |
| 알림은 가는데 옛 값이 보임 | 계산 속성이 값을 저장해 두고 있음 | 오류 없음 |
| 코드에서 `vm.PropertyChanged +=`가 안 옴 | 자식 클래스가 같은 이름의 이벤트를 또 선언해 부모 것을 가림 | 경고 CS0108 |
| 실행하자마자 예외 | 읽기 전용 속성에 TwoWay 바인딩 | `InvalidOperationException` |
| 저장 시 마지막 입력값이 빠짐 | `LostFocus` + 포커스를 받지 않는 버튼 | 오류 없음 |

출력 창 메시지 읽는 법:

```
BindingExpression path error: 'Nmae' property not found on 'object' ''EmployeeViewModel' ...
target element is 'TextBox' ... target property is 'Text'
```

- `'Nmae'`: 찾지 못한 속성 / `'EmployeeViewModel'`: 현재 DataContext / `TextBox`, `Text`: 바인딩을 건 컨트롤과 속성
- VS 2022: 디버그 → 창 → **XAML 바인딩 오류**

---

## 6. Command (ICommand → CommunityToolkit.Mvvm)

목표: 코드비하인드에서 Click 핸들러를 없애고, 버튼이 ViewModel의 메서드를 **바인딩으로** 실행하게 한다. 최종적으로 코드비하인드에는 `DataContext = ...` 한 줄만 남는다.

### 6.1 한눈에 보기

![구성 요약](docs/command-01-overview.png)

- 버튼은 **Command 객체만** 알고, 그 안에 어떤 메서드가 들었는지는 모른다.
- VM의 메서드(`Save`, `LoadAsync`)는 private이어도 된다. 밖에 공개되는 것은 Command 속성뿐이다.
- 버튼, `Ctrl+S`, 메뉴가 **같은 Command 객체**를 공유하면 실행 조건도 하나로 공유된다.

### 6.2 왜 필요한가

- 바인딩은 **속성만** 연결할 수 있고 **메서드는 연결할 수 없다.** (`Click="{Binding Load}"` 불가)
- `ICommand`는 **메서드를 속성으로 포장**해서 바인딩으로 연결할 수 있게 한다.
- 필수는 아니다. 작은 화면은 Click 핸들러에서 `vm.Load()`를 불러도 된다. 다만 아래 경우에 차이가 크다.

| 경우 | Click 핸들러 | Command |
|---|---|---|
| 같은 동작을 버튼, 메뉴, 단축키에 연결 | 핸들러 여러 개 + 각각 Enabled 관리 | Command 하나, 활성 상태 자동 일치 |
| 목록 각 행 안의 버튼 | 어느 행인지 찾아야 함 | `CommandParameter="{Binding}"`로 행 데이터를 받음 |
| 기능 테스트 | 창을 띄워 클릭 | `vm.SaveCommand.Execute(null)` |
| 버튼 상태 관리 | `UpdateUiState()` 같은 코드를 상태 바뀌는 곳마다 호출 | 조건은 `CanExecute` 한 곳, 바뀌면 벨 |

### 6.3 ICommand 구조

.NET이 정해 둔 약속. 멤버는 **3개뿐**이다.

```csharp
public interface ICommand
{
    bool CanExecute(object? parameter);      // 지금 실행해도 되나?
    void Execute(object? parameter);         // 실행해
    event EventHandler? CanExecuteChanged;   // 실행 가능 여부가 바뀌었을 때 울리는 벨
}
```

| 멤버 | 한 줄 뜻 | 누가 부르나 |
|---|---|---|
| `Execute` | 담아 둔 **할 일 메서드**를 실행 | 버튼 (클릭할 때, 활성 상태일 때만) |
| `CanExecute` | 담아 둔 **조건 메서드**를 실행해 `true/false`를 돌려줌 | 버튼 (연결될 때, 벨이 울릴 때, 클릭 직전) |
| `CanExecuteChanged` | **구독자 목록** (이벤트). 버튼들이 "다시묻기" 메서드를 등록해 둔 곳 | 버튼이 구독(`+=`), Command가 울림 |
| `RaiseCanExecuteChanged()` / `NotifyCanExecuteChanged()` | 목록 전원에게 **"다시 물어봐"** 신호. **ICommand 약속에는 없고** RelayCommand가 추가로 제공 | VM (조건이 바뀌었을 때) |

**핵심**: `Execute`와 `CanExecute`는 **우리가 부르지 않는다. 버튼이 부른다.** 우리는 할 일과 조건을 넣어 두고, 조건이 바뀌면 벨만 울린다.

**Delphi `TAction`과 대응**

| Delphi `TAction` | WPF `ICommand` |
|---|---|
| `OnExecute` | `Execute` |
| `OnUpdate`에서 `Enabled := ...` | `CanExecute` |
| idle 때마다 `OnUpdate` 자동 호출 (**물어보러 감**, polling) | `CanExecuteChanged`가 울릴 때만 다시 물음 (**알려 줄 때만**, push) |
| `Button1.Action := actSave` | `Command="{Binding SaveCommand}"` |
| `ShortCut := Ctrl+S` | `KeyBinding Key="S" Modifiers="Ctrl"` |

**버튼 내부에서 일어나는 일** (WPF가 구현해 둔 것, 의사 코드)

```csharp
void OnCommandAttached()                      // Command="{Binding ...}" 연결될 때
{
    Command.CanExecuteChanged += OnCanExecuteChanged;   // 벨 구독
    IsEnabled = Command.CanExecute(null);               // 처음 한 번 물어봄
}
void OnCanExecuteChanged(object? s, EventArgs e)        // 벨이 울리면
    => IsEnabled = Command.CanExecute(null);            // 다시 물어봄
void OnClick()
{
    if (Command.CanExecute(null)) Command.Execute(null);
}
```

### 6.4 RelayCommand: 메서드를 담아 두는 상자

명령마다 `ICommand` 클래스를 만들지 않도록, 메서드를 받아서 **대신 호출해 주는(relay)** 범용 클래스.

```csharp
public class RelayCommand : ICommand
{
    private readonly Action _execute;           // 할 일 (Delphi: procedure of object)
    private readonly Func<bool>? _canExecute;   // 조건 (Delphi: function: Boolean of object), 생략 가능

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? p) => _canExecute?.Invoke() ?? true;   // 조건 없으면 항상 true
    public void Execute(object? p)    => _execute();
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
```

| 문법 | 뜻 |
|---|---|
| `Action` | 값을 받지 않고 돌려주지 않는 메서드를 담는 타입. `Action a = Load;` (괄호 없음 = 메서드 자체) → `a();` |
| `Action<object?>` | 값 하나를 받는 메서드를 담는 타입 (CommandParameter용) |
| `Func<bool>` | `bool`을 돌려주는 메서드를 담는 타입 |
| `?.Invoke()` | null이면 호출하지 않음 (Delphi `if Assigned(...) then`) |
| `?? true` | 왼쪽이 null이면 `true` |
| `EventArgs.Empty` | 보낼 추가 정보 없음. 이벤트 형식이 (보낸 쪽, 추가 정보) 두 칸이라 빈 값을 넣음 |

**`RaiseCanExecuteChanged`가 따로 있는 이유**: `event`는 바깥에서 `+=`, `-=`만 할 수 있고 **호출은 선언한 클래스 안에서만** 된다. VM에서 `SaveCommand.CanExecuteChanged(...)`는 컴파일 오류라서, RelayCommand가 "밖에서 울리는 손잡이"를 열어 둔다. (`ObservableObject.OnPropertyChanged`와 같은 이유)

### 6.5 호출 흐름 ① 연결과 클릭

![연결과 클릭](docs/command-02-connect-click.png)

```
화면이 뜰 때:  버튼 → CanExecuteChanged 구독 → CanExecute() → CanSaveNow() → true → 켜짐
클릭할 때:    버튼 → CanExecute() 재확인 → Execute() → Save() → StatusMessage 변경 → (PropertyChanged) → 상태바 갱신
```

### 6.6 호출 흐름 ② 조건이 바뀔 때 (벨)

![조건 변경과 벨](docs/command-03-canexecute-bell.png)

**벨이 없으면**: 버튼은 화면이 뜰 때 물어본 답을 계속 쓴다. 이름을 지워도 저장 버튼이 켜진 채로 남고, 클릭하면 `CanExecute` 재확인에서 막혀 **켜져 보이는데 아무 일도 안 일어나는** 상태가 된다.

**INotifyPropertyChanged와 같은 구조**

| | `INotifyPropertyChanged` | `ICommand` |
|---|---|---|
| 화면이 읽는 것 | 속성 값 | `CanExecute()` 결과 |
| 화면이 구독하는 벨 | `PropertyChanged` | `CanExecuteChanged` |
| 벨 손잡이 | `OnPropertyChanged()` | `NotifyCanExecuteChanged()` |
| 화면 → VM | TwoWay로 setter 호출 | 클릭 시 `Execute()` |

**CanExecute와 IsEnabled**

- 버튼의 최종 활성 = **XAML의 `IsEnabled` AND `CanExecute` 결과.** 둘 중 하나라도 false면 꺼진다. Command를 쓰면 `IsEnabled`는 따로 쓰지 않고 조건을 `CanExecute` 한 곳에 둔다.
- `Visibility`는 바꾸지 않는다 (숨기려면 별도 바인딩).
- `KeyBinding`은 보이는 게 없으니 `false`면 **단축키를 눌러도 실행 안 됨**으로 나타난다.

**Command를 받는 컨트롤** (`ICommandSource`)

| 받음 | 안 받음 |
|---|---|
| `Button`, `ToggleButton`, `CheckBox`, `RadioButton`, `RepeatButton`, `MenuItem`, `Hyperlink`, `KeyBinding`, `MouseBinding` | `TextBox`, `ComboBox`, `DatePicker`, `DataGrid`, `ListBox` 등 입력/선택 컨트롤 |

입력 컨트롤을 끄려면 `IsEnabled="{Binding bool속성}"` (바인딩 방식). **조건은 VM 한 곳, 연결 방식만 다르게.**

### 6.7 VM에서 화면 기능 빼기

- VM에 `MessageBox.Show`가 있으면 테스트 도중 창이 떠서 멈춘다. `using System.Windows;`가 VM에 있으면 화면 의존 신호.
- 알릴 내용은 **VM 속성**(`StatusMessage`)에 넣고 화면이 바인딩으로 보여 준다.
- 같은 문자열을 두 번 넣으면 `SetProperty`가 알림을 보내지 않는다 → 시각 등 매번 달라지는 값을 넣는다.
- 확인 창이 꼭 필요하면 VM은 `IDialogService` 같은 **인터페이스만** 받아서 쓴다 (DI 항목).
- `ICommand`는 `System.Windows.Input` 네임스페이스지만 WPF가 아니라 .NET 기본 라이브러리 타입이라 화면 의존이 아니다.

### 6.8 CommandParameter와 KeyBinding

```xml
<Button Command="{Binding NavigateCommand}" CommandParameter="직원 목록"/>

<Window.InputBindings>
    <KeyBinding Key="S" Modifiers="Ctrl" Command="{Binding SaveCommand}"/>
</Window.InputBindings>
```

- `CommandParameter` 값이 `Execute(parameter)`의 `parameter`로 들어온다. Delphi의 공용 `OnClick` + `(Sender as TButton).Tag`를 `Sender` 없이 하는 것.
- XAML에 글자로 쓰면 `string`, 안 쓰면 `null`. 목록 행에서는 `{Binding}`으로 행 객체가 통째로 들어온다.
- 받는 쪽 검사: `if (menu is not string menuName) return;` → string이면 새 지역 변수 `menuName`에 담고, 아니면(null 포함) 종료. `RelayCommand<string>`이면 `if (menu is null) return;`만.
- **단축키는 포커스를 옮기지 않는다.** `LostFocus` 바인딩 칸에 커서를 둔 채 `Ctrl+S` → 이전 값이 저장된다 (바인딩 차시 `Focusable=False` 실험과 같음). 텍스트 입력칸은 `UpdateSourceTrigger=PropertyChanged`로 맞춘다.

### 6.9 CommunityToolkit.Mvvm 교체

| 직접 만든 것 | 라이브러리 | 비고 |
|---|---|---|
| `ObservableObject` | `ObservableObject` (`CommunityToolkit.Mvvm.ComponentModel`) | `SetProperty`, `OnPropertyChanged` 이름 같음 |
| `RelayCommand` | `RelayCommand` (`CommunityToolkit.Mvvm.Input`) | `RaiseCanExecuteChanged()` → **`NotifyCanExecuteChanged()`** |
| `RelayParamCommand` (`object?`) | `RelayCommand<T>` | 매개변수 타입 지정, 형변환 불필요 |
| (없음) | `AsyncRelayCommand` | `async Task` 메서드용 |

- `NotifyCanExecuteChanged()`는 `ICommand`에 없으므로 속성 타입은 `RelayCommand` 또는 `IRelayCommand`로.
- 이름을 라이브러리와 맞춰 두면 `using`만 바꿔 대부분 그대로 동작한다.

### 6.10 소스 생성기 (반복 코드 제거)

```csharp
public partial class EmployeeViewModel : ObservableObject      // partial 필수
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Summary))]                 // Name 바뀌면 Summary도 알림
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]           // Name 바뀌면 SaveCommand 벨
    private string _name = "김민수";                             // → Name 속성 생성

    public string Summary => $"편집 중: {Name} ({EmployeeNo}) 부서: {Department}";

    [RelayCommand(CanExecute = nameof(CanSaveNow))]             // → SaveCommand 생성
    private void Save() { ... }
    private bool CanSaveNow() => !string.IsNullOrWhiteSpace(Name) && !IsLoading;

    [RelayCommand]                                              // → NavigateCommand (RelayCommand<string>)
    private void Navigate(string? menu) { ... }
}
```

| 붙이는 것 | 생성되는 것 / 하는 일 |
|---|---|
| `[ObservableProperty] _name` | `Name` 속성 (`_` 떼고 첫 글자 대문자). setter는 `SetProperty` |
| `[NotifyPropertyChangedFor(nameof(X))]` | 이 속성이 바뀌면 `X`도 PropertyChanged |
| `[NotifyCanExecuteChangedFor(nameof(XCommand))]` | 이 속성이 바뀌면 `XCommand.NotifyCanExecuteChanged()` |
| `[RelayCommand] void Save()` | `SaveCommand` (`RelayCommand`) |
| `[RelayCommand] void Navigate(string? m)` | `NavigateCommand` (`RelayCommand<string>`, 매개변수 타입을 보고 결정) |
| `[RelayCommand] async Task LoadAsync()` | `LoadCommand` (`AsyncRelayCommand`, **`Async` 접미사를 뗌**) |
| `CanExecute = nameof(메서드)` | 조건 메서드 연결 |

**주의**

- 생성된 코드는 F12로 볼 수 있다 (`EmployeeViewModel.g.cs`, 분석기 → `CommunityToolkit.Mvvm.SourceGenerators`). 빌드마다 다시 만들어지므로 **직접 고칠 수 없다.** `partial`은 우리 파일과 생성 파일을 한 클래스로 합치기 위한 것.
- 초기값은 필드 선언에서 넣는다. 코드에서는 **필드(`_name`)가 아니라 속성(`Name`)**을 쓴다. 필드에 바로 대입하면 setter를 거치지 않아 알림/벨이 없다.
- **연관 알림 누락은 다른 알림에 가려진다.** `_employeeNo`에 `[NotifyPropertyChangedFor(nameof(Summary))]`가 빠졌어도, 불러오기는 이름도 같이 바꾸니 Summary가 갱신되어 보였다. 사번만 따로 바꿔 봐야 드러난다.
- 읽기 전용 목록에 `[ObservableProperty]`를 붙이면 setter가 생겨 열린다. 그대로 `public IReadOnlyList<string> Departments { get; } = ...`로 둔다.

### 6.11 호출 흐름 ③ 비동기 Command와 취소

![비동기 실행과 취소](docs/command-04-async-cancel.png)

```csharp
[ObservableProperty]
[NotifyCanExecuteChangedFor(nameof(SaveCommand))]
private bool _isLoading;

[RelayCommand(IncludeCancelCommand = true)]                     // → LoadCommand + LoadCancelCommand
private async Task LoadAsync(CancellationToken ct)
{
    IsLoading = true;                                           // 속성으로! (필드면 벨 없음)
    try
    {
        StatusMessage = "불러오는 중 ...";
        await Task.Delay(2000, ct);                             // ct를 넘겨야 취소가 먹힌다
        Name = "홍길동"; ...
        StatusMessage = $"불러옴: {Name}";
    }
    catch (OperationCanceledException)
    {
        StatusMessage = "취소됨";                                // 대입 코드는 건너뜀
    }
    finally
    {
        IsLoading = false;                                      // 성공, 취소, 예외 모두
    }
}
```

```xml
<ProgressBar IsIndeterminate="{Binding LoadCommand.IsRunning}" Width="100" Height="12"/>
<Button Content="취소" Command="{Binding LoadCancelCommand}"/>
```

**라이브러리가 대신 해 주는 것** (비동기 차시에서 직접 했던 것)

| 비동기 차시 (직접) | AsyncRelayCommand |
|---|---|
| 조회 중 버튼 비활성화 (`UpdateUiState`) | 실행 중 `CanExecute = false` + 시작/끝에 벨 (기본: 중복 실행 금지) |
| `btnCancel.Enabled = searching;` | `LoadCancelCommand.CanExecute` = 실행 중이고 취소 전 |
| `_cts` 생성, `Dispose`, 최신 요청 비교 | 실행마다 토큰 자동 생성, 취소 Command가 `Cancel()` |
| 진행 막대 표시/숨김 | `IsRunning` 바인딩 |

**라이브러리가 안 해 주는 것**

- **토큰 전달.** `Task.Delay(2000)`처럼 `ct`를 안 넘기면 취소 버튼을 눌러도 끝까지 실행된다. 취소는 강제 종료가 아니라 협조 요청.
- **다른 Command의 벨.** `LoadCommand.IsRunning`을 `CanSaveNow()` 조건으로 쓰면, IsRunning이 바뀌어도 `SaveCommand` 벨은 아무도 안 울린다. 그래서 `IsLoading`을 VM에 두고 `[NotifyCanExecuteChangedFor(nameof(SaveCommand))]`로 연결한다.
- `AllowConcurrentExecutions = true`로 중복 실행을 허용할 수 있다.

### 6.12 겪은 오류와 원인

| 증상 / 오류 | 원인 | 해결 |
|---|---|---|
| `new RelayCommand(Load)` 컴파일 오류 | 조건 매개변수가 필수 | `Func<bool>? canExecute = null` |
| `_execute()`에서 null 경고 CS8602 | 필수인 할 일 필드를 `Action?`로 선언 | `readonly Action` (`?`는 생략 가능한 조건에만) |
| 이름을 지워도 저장 버튼이 켜져 있음 | 벨을 울리지 않음 | setter에서 `NotifyCanExecuteChanged()` / `[NotifyCanExecuteChangedFor]` |
| 속성 타입을 `RelayCommand`로 바꾸자 CS0053 | `RelayCommand` 클래스가 internal인데 public 속성 타입으로 씀 | `public class RelayCommand` |
| 버튼이 아무 반응 없음 | `SaveComamand` 오타 → XAML이 `SaveCommand`를 못 찾음 (조용한 path error) | 이름 일치 |
| 교체 후 CS1061 `RaiseCanExecuteChanged` 없음 | 라이브러리는 이름이 다름 | `NotifyCanExecuteChanged()` |
| 상태 메시지가 계속 "준비" | `get => _statusMessage; set;` → setter는 숨은 필드에 저장, getter는 다른 필드를 읽음 + 알림 없음 | `set => SetProperty(ref _statusMessage, value);` |
| 진행 막대를 추가하자 Summary가 왼쪽으로 이동 | StatusBar는 DockPanel, **마지막 아이템만** 남은 공간 차지 | 오른쪽에 둘 아이템을 마지막에 |
| 진행 막대가 안 움직임 | `isRunning` 소문자 (출력 창에 `AsyncRelayCommand` 타입도 보임) | `IsRunning` |
| 취소가 안 됨 | `Task.Delay(2000)`에 `ct` 미전달 | `Task.Delay(2000, ct)` |
| 불러오는 중에도 저장이 눌림 | `IsLoading`을 선언만 하고 바꾸지 않음 | 시작에 `true`, `finally`에 `false` |

---

## 다음

- `ObservableCollection` + DataGrid/ListView 목록 화면 ("직원 목록" 메뉴), 행 버튼에 `CommandParameter="{Binding}"`
