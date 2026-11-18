using System.ComponentModel;
using System.Diagnostics;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        ITicketRepository ticketRepository;
        private CancellationTokenSource? _cts;
        private CancellationTokenSource? _saveCts;

        public Form1()
        {
            InitializeComponent();
            ticketRepository = new FakeTicketRepository(true);
            UpdateUiState();
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            bool isCanceled = false;
            toolStripStatusLabel1.Text = "조회 중...";

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            var cts = _cts;
            var ct = _cts.Token;

            UpdateUiState();

            try
            {
                var timer = new Stopwatch();
                timer.Start();

                var result = await ticketRepository.GetTicketsAsync(txtKeyword.Text, ct);
                ct.ThrowIfCancellationRequested();
                bindingSource1.DataSource = new BindingList<Ticket>(result.ToList());
                timer.Stop();
                toolStripStatusLabel2.Text = $"마지막 조회: {result.Count.ToString()}건, {timer.Elapsed.TotalSeconds:F1}초";
            }
            catch (OperationCanceledException)
            {
                isCanceled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (ReferenceEquals(_cts, cts))
                {                    
                    _cts = null;
                    cts.Dispose();                    
                    if (!IsDisposed) {
                        toolStripStatusLabel1.Text = isCanceled ? "취소됨" : "";
                        UpdateUiState();
                    }
                }
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {

            dgvTickets.EndEdit();
            bindingSource1.EndEdit();            

            _saveCts?.Cancel();
            _saveCts?.Dispose();
            _saveCts = new CancellationTokenSource();
            var cts = _saveCts;
            var ct = _saveCts.Token;

            UpdateUiState();

            try
            {
                var timer = new Stopwatch();
                timer.Start();

                var snapshot = bindingSource1.List.Cast<Ticket>().ToList();
                await ticketRepository.SaveAsync(snapshot, ct);

                timer.Stop();
                toolStripStatusLabel2.Text = $"저장 완료: {snapshot.Count.ToString()}건, {timer.Elapsed.TotalSeconds:F1}초";
            }
            catch (OperationCanceledException)
            {
                if (IsDisposed) return;
                toolStripStatusLabel2.Text = "취소됨";
            }
            catch (Exception ex)
            {
                if (IsDisposed) return;
                MessageBox.Show(ex.Message);
            }
            finally
            {                
                _saveCts = null;
                cts.Dispose();
                UpdateUiState();
            }

        }


        private void UpdateUiState()
        {
            if (IsDisposed) return;
            bool searching = _cts != null;
            bool saving = _saveCts != null;

            btnSearch.Enabled = !saving;
            btnSave.Enabled = !searching && !saving;
            btnCancel.Enabled = searching;
            dgvTickets.ReadOnly = searching || saving;
            toolStripProgressBar1.Visible = searching || saving;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_saveCts != null)
            {
                var result = MessageBox.Show("저장 중입니다. 닫으시겠습니까?","확인",MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes) { _saveCts?.Cancel(); }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }else if (_cts != null)
            {
                _cts.Cancel();
                return;
            }
            
        }
    }
}
