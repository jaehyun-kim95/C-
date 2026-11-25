# WPF 학습 노트: XAML 레이아웃과 데이터 바인딩

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

## 다음

- `ICommand` 직접 구현 → CommunityToolkit.Mvvm으로 교체 (코드비하인드 Click 핸들러를 ViewModel로)
