using System.Diagnostics;
using static WinFormsApp1.Validator;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {

        Validator.FormValidator validator = new Validator.FormValidator();
        private bool _isDone;

        public Form1()
        {
            InitializeComponent();
            txtDepositor.Enabled = false;

            foreach (var tb in new[] { txtReceiver, txtPhone, txtZip, txtAddress, txtDepositor })
                tb.TextChanged += Input_Changed;

            dtpDelivery.ValueChanged += Input_Changed;
            cboPayment.SelectedIndexChanged += Input_Changed;

            UpdateSubmitButton();

            FormClosing += Form1_FormClosing;
        }

        private void txtReceiver_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (txtReceiver.Text.Length < 2 || txtReceiver.Text.Length > 20)
            {
                errorProvider1.SetError(txtReceiver, "이름은 2~20자로 입력하세요");
                e.Cancel = true;
            }
            else
            {
                errorProvider1.SetError(txtReceiver, "");
            }
        }

        private void txtPhone_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string? isValidate = validator.ValidatePhone(txtPhone.Text);
            errorProvider1.SetError(txtPhone, isValidate ?? "");
            if (isValidate is not null)
            {
                e.Cancel = true;
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren())
            {
                Control[] ordered = { txtReceiver, txtPhone, txtZip, txtAddress, dtpDelivery, cboPayment, txtDepositor };
                var invalid = ordered.Where(c => errorProvider1.GetError(c) != "").ToList();

                string summary = string.Join(Environment.NewLine, invalid.Select(c => "· " + errorProvider1.GetError(c)));
                MessageBox.Show(summary, "입력 확인", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                invalid.FirstOrDefault()?.Focus();
                return;
            }

            try
            {
                SaveShipping();

                MessageService.ShowInfo(
                    $"받는 사람: {txtReceiver.Text.Trim()}{Environment.NewLine}" +
                    $"주소: ({txtZip.Text}) {txtAddress.Text.Trim()}{Environment.NewLine}" +
                    $"배송일: {dtpDelivery.Value:yyyy-MM-dd}{Environment.NewLine}" +
                    $"결제: {cboPayment.Text}");

                _isDone = true;
                DialogResult = DialogResult.OK;   // 폼을 닫는다
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);   // 상세 내용은 개발자용 출력으로만
                MessageService.ShowError("신청 처리 중 문제가 발생했습니다. 잠시 후 다시 시도해 주세요.");
            }
        }
        private void SaveShipping()
        {

        }

        private void dtpDelivery_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string? error = validator.ValidateDeliveryDate(dtpDelivery.Value);
            errorProvider1.SetError(dtpDelivery, error ?? "");
            e.Cancel = error is not null;
        }

        private void txtDepositor_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !CheckDepositor();
        }

        private void cboPayment_SelectedIndexChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(cboPayment, "");

            bool isBank = cboPayment.Text == "무통장";
            txtDepositor.Enabled = isBank;
            if (!isBank)
            {
                txtDepositor.Clear();
                errorProvider1.SetError(txtDepositor, "");
            }
            else
            {
                CheckDepositor();
            }
        }

        private bool CheckDepositor()
        {
            string? error = validator.ValidateDepositor(cboPayment.Text, txtDepositor.Text);
            errorProvider1.SetError(txtDepositor, error ?? "");
            return error is null;
        }

        private bool IsFormValid()
        {
            return validator.ValidatePhone(txtPhone.Text) is null
                && validator.ValidateDeliveryDate(dtpDelivery.Value) is null
                && validator.ValidateDepositor(cboPayment.Text, txtDepositor.Text) is null;
        }

        private void UpdateSubmitButton()
        {
            btnSubmit.Enabled = IsFormValid();
        }

        private void Input_Changed(object? sender, EventArgs e)
        {
            if (sender is Control c)
                errorProvider1.SetError(c, "");

            UpdateSubmitButton();
        }

        private bool HasInput() =>
            txtReceiver.Text.Trim() != "" || txtPhone.Text != "" || txtZip.Text != ""
            || txtAddress.Text.Trim() != "" || cboPayment.SelectedIndex >= 0 || txtDepositor.Text.Trim() != "";

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_isDone || !HasInput()) return;                          // 끝났거나 입력이 없으면 묻지 않음
            if (e.CloseReason != CloseReason.UserClosing) return;        // 윈도우 종료 등에서는 묻지 않음

            if (!MessageService.Confirm("입력한 내용이 사라집니다. 닫을까요?"))
                e.Cancel = true;                                         // 닫기를 막는다
        }
    }
}
