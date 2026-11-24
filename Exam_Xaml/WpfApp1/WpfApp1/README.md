# XAML 레이아웃 (직원 정보 등록 화면, WPF)

## 의도

WPF의 레이아웃 컨트롤(`Grid`, `StackPanel`, `Border`, `ScrollViewer`)을 조합해 **창 크기가 바뀌어도 무너지지 않는 화면**을 만들고, `Style` 리소스로 반복되는 속성을 한 곳에서 관리하는 방법을 익힌다.

## 작업한 것

| 항목 | 내용 |
|---|---|
| 전체 골격 | 최상위 `Grid` 4행: 헤더(50) / 본문(`*`) / 버튼 영역(`Auto`) / `StatusBar`(`Auto`) |
| 헤더 | `Border` + 2열 `Grid`(`*`, `Auto`)로 제목은 왼쪽, 사용자 정보는 오른쪽 배치 |
| 본문 | 2열 `Grid`: 왼쪽 메뉴(180 고정) / 오른쪽 폼(`*`) |
| 메뉴 | `StackPanel` 세로 나열, `ScrollViewer`로 창이 작을 때 스크롤 |
| 입력 폼 | 라벨(100 고정) + 입력(`*`) 2열, 항목별 `Auto` 행. 사번·이름·부서·입사일·이메일·메모 |
| 스타일 | `MenuButton`, `SelectedButton`(`BasedOn` 상속), `FormLabel`, `FormTextBox`, `FormCombo`, `FormDate` |
| 하단 | 저장/취소 버튼은 `StackPanel`(가로)을 오른쪽 정렬, 상태 표시줄은 `StatusBar` |
| 창 설정 | `WindowStartupLocation="CenterScreen"`, `MinWidth`/`MinHeight` 지정 |

## 중요한 포인트 3가지

1. **`Grid`의 행/열 크기 단위를 용도별로 고른다**
   - `Auto`는 내용만큼, `*`는 남은 공간 전부, 숫자는 고정 크기다.
   - 헤더·버튼 영역은 고정/`Auto`, 본문은 `*`로 두어 창을 늘리면 본문만 늘어난다.
   - 폼의 마지막 행(메모)을 `*`로 두면 남은 높이를 메모 입력란이 차지한다.

2. **`Style`로 반복을 없애고 `BasedOn`으로 확장한다**
   - `Window.Resources`에 `x:Key`로 정의하고 `{StaticResource ...}`로 적용한다.
   - `SelectedButton`은 `MenuButton`을 `BasedOn`으로 상속해 배경·굵기만 덮어쓴다.
   - 컨트롤에 직접 쓴 속성(예: 이메일의 `HorizontalAlignment="Stretch"`)은 스타일보다 우선한다.

3. **정렬과 크기 속성을 구분해서 쓴다**
   - `Margin`은 바깥 여백, `Padding`은 안쪽 여백이다.
   - `HorizontalAlignment`/`VerticalAlignment`는 칸 안에서의 위치, `HorizontalContentAlignment`/`VerticalContentAlignment`는 컨트롤 내용의 위치다.
   - 입력란에 `Width`를 주고 `Left`로 정렬하면 고정 폭, `Stretch`면 칸 폭을 따라간다.
   - 레이아웃 컨테이너(`Grid`, `StackPanel`)에는 `Grid.Row`를 중복 지정하지 않고 자식 요소에서만 지정한다.
