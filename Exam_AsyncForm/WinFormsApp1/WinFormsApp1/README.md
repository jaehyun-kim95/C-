# async/await로 UI 멈춤 방지 (문의 티켓 관리, WinForms)

## 의도

조회·저장처럼 오래 걸리는 작업을 `async/await`로 바꿔 **UI가 멈추지 않게** 하고, 취소·중복 요청·종료 같은 실무 상황을 안전하게 처리하는 방법을 익힌다.

## 작업한 것

| 항목 | 내용 |
|---|---|
| 구성 | `Ticket` / `ITicketRepository` / `FakeTicketRepository`(지연·무작위 지연 재현) / `Form1` |
| 비동기 전환 | 조회·저장을 `async Task`로 만들고 버튼 핸들러에서 `await` |
| 로딩 표시 | Marquee 진행 막대, 버튼 활성/비활성, 상태 문구 |
| 취소 | `CancellationTokenSource` 요청별 생성, 취소 버튼, 재조회 시 이전 요청 취소 |
| 저장 | 별도 `_saveCts`, 그리드 편집 확정(`EndEdit`) 후 스냅샷을 저장 |
| UI 상태 | `UpdateUiState()` 한 곳에서 조회/저장 상태로 컨트롤 상태 계산 |
| 종료 처리 | `FormClosing`에서 저장 중 확인, 조회 중 취소 |
| 예외 처리 | 핸들러 `try/catch/finally`, `async void` vs `async Task` 정리 (이론) |

## 중요한 포인트 3가지

1. **`await`는 UI 스레드를 막지 않고, 끝나면 UI 스레드로 돌아온다**
   - `Thread.Sleep`은 UI가 멈추고 `await Task.Delay`는 멈추지 않는다. 전후 스레드 ID가 같음을 직접 확인했다.
   - UI 코드에서 `.Result`/`.Wait()`는 교착 위험이 있으니 쓰지 않고 끝까지 `await`한다.

2. **"마지막 요청만 반영"은 요청별 상태로 처리한다**
   - 공유 bool 플래그 대신 요청마다 `cts`와 지역 변수를 만들고, `finally`에서 `ReferenceEquals(_cts, cts)`로 내 요청일 때만 정리한다.
   - `await` 뒤에 `ct.ThrowIfCancellationRequested()`를 두어 취소된 요청의 늦은 결과가 화면에 덮어쓰지 못하게 한다.
   - `OperationCanceledException`은 오류가 아니므로 메시지 없이 처리한다.

3. **예외와 UI 복구는 정해진 자리에서 한다**
   - 이벤트 핸들러만 `async void`이며 그 안에서 `try/catch`로 모두 처리한다. 일반 메서드는 `async Task`로 만들고 예외를 그대로 전파한다.
   - 복구는 `finally`, 알림은 `catch`에 둔다. 컨트롤 상태는 `UpdateUiState()`로만 바꿔 조회·저장 동시 동작에서도 어긋나지 않게 한다.
