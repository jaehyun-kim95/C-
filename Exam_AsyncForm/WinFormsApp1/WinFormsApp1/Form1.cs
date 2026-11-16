using System.ComponentModel;
using System.Diagnostics;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        ITicketRepository ticketRepository;
        private CancellationTokenSource? _cts;

        public Form1()
        {
            InitializeComponent();
            ticketRepository = new FakeTicketRepository(true);
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            bool isCanceled = false;
            btnCancel.Enabled = true;
            toolStripStatusLabel1.Text = "조회 중...";
            toolStripProgressBar1.Visible = true;

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            var cts = _cts;
            var ct = _cts.Token;


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
            catch (OperationCanceledException) {
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
                    toolStripStatusLabel1.Text = isCanceled ? "취소됨" : "";                    
                    toolStripProgressBar1.Visible = false;
                    _cts = null;
                    cts.Dispose();
                    btnCancel.Enabled = false;
                }
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();        
        }
    }
}
